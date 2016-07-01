using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.SqliteWrapper.Repositories;
using Famoser.SqliteWrapper.Services.Interfaces;
using Newtonsoft.Json;
using Nito.AsyncEx;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Helpers.Text;
using OfflineMedia.Business.Managers;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.ContentModels;
using OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;
using OfflineMedia.Business.Repositories.Base;
using OfflineMedia.Business.Repositories.Interfaces;
using OfflineMedia.Business.Services;
using OfflineMedia.Data.Entities.Database;
using OfflineMedia.Data.Entities.Database.Contents;
using OfflineMedia.Data.Entities.Database.Relations;
using OfflineMedia.Data.Entities.Storage;
using OfflineMedia.Data.Entities.Storage.Settings;
using OfflineMedia.Data.Entities.Storage.Sources;
using OfflineMedia.Data.Enums;

namespace OfflineMedia.Business.Repositories
{
    public partial class ArticleRepository : BaseRepository, IArticleRepository
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IProgressService _progressService;
        private readonly IStorageService _storageService;
        private readonly ISqliteService _sqliteService;
        private readonly IPlatformCodeService _platformCodeService;
        private readonly ImageDownloadService _imageDownloadService;

        private readonly GenericRepository<ArticleModel, ArticleEntity> _articleGenericRepository;
        private readonly GenericRepository<ImageContentModel, ImageContentEntity> _imageContentGenericRepository;
        private readonly GenericRepository<TextContentModel, TextContentEntity> _textContentGenericRepository;
        private readonly GenericRepository<GalleryContentModel, GalleryContentEntity> _galleryContentGenericRepository;

#pragma warning disable 4014
        public ArticleRepository(ISettingsRepository settingsRepository, IProgressService progressService, IStorageService storageService, ISqliteService sqliteService, IPlatformCodeService platformCodeService)
        {
            _settingsRepository = settingsRepository;
            _progressService = progressService;
            _storageService = storageService;
            _sqliteService = sqliteService;
            _platformCodeService = platformCodeService;

            _articleGenericRepository = new GenericRepository<ArticleModel, ArticleEntity>(_sqliteService);
            _imageContentGenericRepository = new GenericRepository<ImageContentModel, ImageContentEntity>(_sqliteService);
            _textContentGenericRepository = new GenericRepository<TextContentModel, TextContentEntity>(_sqliteService);
            _galleryContentGenericRepository = new GenericRepository<GalleryContentModel, GalleryContentEntity>(_sqliteService);
            _imageDownloadService = new ImageDownloadService(_platformCodeService, _sqliteService);

            Initialize();
        }

        public ObservableCollection<SourceModel> GetActiveSources()
        {
            Initialize();
            return SourceManager.GetActiveSources();
        }

        public ObservableCollection<SourceModel> GetAllSources()
        {
            Initialize();
            return SourceManager.GetAllSources();
        }
#pragma warning restore 4014

        private bool _isInitialized;
        private readonly AsyncLock _initializeAsyncLock = new AsyncLock();
        private SourceCacheEntity _sourceCacheEntity;
        private Task Initialize()
        {
            return ExecuteSafe(async () =>
            {
                using (await _initializeAsyncLock.LockAsync())
                {
                    if (_isInitialized)
                        return;

                    var jsonAssets = await _storageService.GetAssetTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.SourcesConfiguration).Description);
                    var feeds = JsonConvert.DeserializeObject<List<SourceEntity>>(jsonAssets);

                    try
                    {
                        var json = await _storageService.GetCachedTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.SettingsUserConfiguration).Description);

                        if (!string.IsNullOrEmpty(json))
                            _sourceCacheEntity = JsonConvert.DeserializeObject<SourceCacheEntity>(json);
                        else
                            _sourceCacheEntity = new SourceCacheEntity();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.LogException(ex);
                        _sourceCacheEntity = new SourceCacheEntity();
                    }

                    var feedsToLoad = new List<FeedModel>();
                    foreach (var sourceEntity in feeds)
                    {
                        var source = EntityModelConverter.Convert(sourceEntity);
                        if (!_sourceCacheEntity.IsEnabledDictionary.ContainsKey(source.Guid))
                            _sourceCacheEntity.IsEnabledDictionary[source.Guid] = false;

                        SourceManager.AddSource(source, _sourceCacheEntity.IsEnabledDictionary[source.Guid]);
                        foreach (var feedEntity in sourceEntity.Feeds)
                        {
                            if (!_sourceCacheEntity.IsEnabledDictionary.ContainsKey(feedEntity.Guid))
                                _sourceCacheEntity.IsEnabledDictionary[feedEntity.Guid] = false;

                            var feed = EntityModelConverter.Convert(feedEntity, source, _sourceCacheEntity.IsEnabledDictionary[feedEntity.Guid]);
                            SourceManager.AddFeed(feed, source, _sourceCacheEntity.IsEnabledDictionary[source.Guid]);

                            if (_sourceCacheEntity.IsEnabledDictionary[feedEntity.Guid])
                                feedsToLoad.Add(feed);
                        }
                    }

                    foreach (var feedModel in feedsToLoad)
                    {
                        //todo: remove await? not sure if sqlite is threadsafe
                        //if so, remember to do LoadArticlesIntoToFeed Safe
                        await LoadArticlesIntoToFeed(feedModel, 5);
                    }

                    _isInitialized = true;
                }
            });
        }

        private async Task LoadArticlesIntoToFeed(FeedModel feed, int max)
        {
            var relations = await _sqliteService.GetByCondition<FeedArticleRelationEntity>(s => s.FeedGuid == feed.Guid.ToString(), s => s.Index, false, max, feed.ArticleList.Count);
            foreach (var feedArticleRelationEntity in relations)
            {
                var article = await _articleGenericRepository.GetByIdAsync(feedArticleRelationEntity.ArticleId);
                feed.ArticleList.Add(article);
                var contents = await _sqliteService.GetByCondition<ContentEntity>(s => s.ParentId == article.GetId() && s.ContentType == (int)ContentType.LeadImage, s => s.Index, false, 1, 0);
                if (contents?.FirstOrDefault() != null)
                {
                    var image = await _imageContentGenericRepository.GetByIdAsync(contents.FirstOrDefault().ContentId);
                    article.LeadImage = image;
                }
            }
        }

        public Task<bool> LoadFullArticleAsync(ArticleModel am)
        {
            return ExecuteSafe(async () =>
            {
                var contents = await _sqliteService.GetByCondition<ContentEntity>(s => s.ParentId == am.GetId(), s => s.Index, false, 0, 0);
                foreach (var contentEntity in contents)
                {
                    switch (contentEntity.ContentType)
                    {
                        case (int)ContentType.Text:
                            {
                                var text = await _textContentGenericRepository.GetByIdAsync(contentEntity.ContentId);
                                text.Content = JsonConvert.DeserializeObject<ObservableCollection<ParagraphModel>>(text.ContentJson);
                                am.Content.Add(text);
                                break;
                            }
                        case (int)ContentType.Image:
                            {
                                var image = await _imageContentGenericRepository.GetByIdAsync(contentEntity.ContentId);
                                am.Content.Add(image);
                                break;
                            }
                        case (int)ContentType.Gallery:
                            {
                                var galleryContents = await _sqliteService.GetByCondition<ContentEntity>(s => s.ParentId == am.GetId(), s => s.Index, false, 0, 0);
                                var gallery = await _galleryContentGenericRepository.GetByIdAsync(contentEntity.ContentId);
                                am.Content.Add(gallery);

                                foreach (var galleryContent in galleryContents.Where(g => g.ContentType == (int)ContentType.Image))
                                {
                                    var image = await _imageContentGenericRepository.GetByIdAsync(galleryContent.ContentId);
                                    gallery.Images.Add(image);
                                }
                                break;
                            }
                    }
                }


                return true;
            });
        }

        public Task<bool> LoadFullFeedAsync(FeedModel fm)
        {
            return ExecuteSafe(async () =>
            {
                await LoadArticlesIntoToFeed(fm, 0);

                return true;
            });
        }

        public Task<bool> SetArticleFavoriteStateAsync(ArticleModel am, bool isFavorite)
        {
            return ExecuteSafe(async () =>
            {
                am.IsFavorite = isFavorite;
                return await _articleGenericRepository.UpdateAsyc(am);
            });
        }

        public Task<bool> MarkArticleAsReadAsync(ArticleModel am)
        {
            return ExecuteSafe(async () =>
            {
                am.IsRead = true;
                return await _articleGenericRepository.UpdateAsyc(am);
            });
        }

        public Task<bool> SetFeedActiveStateAsync(FeedModel feedModel, bool isActive)
        {
            SourceManager.SetFeedActiveState(feedModel, isActive);
            _sourceCacheEntity.IsEnabledDictionary[feedModel.Guid] = isActive;
            return SaveCache();
        }

        public Task<bool> SetSourceActiveStateAsync(SourceModel sourceModel, bool isActive)
        {
            SourceManager.SetSourceActiveState(sourceModel, isActive);
            _sourceCacheEntity.IsEnabledDictionary[sourceModel.Guid] = isActive;
            return SaveCache();
        }

        private Task<bool> SaveCache()
        {
            return ExecuteSafe(async () =>
            {
                var json = JsonConvert.SerializeObject(_sourceCacheEntity);
                return await _storageService.SetCachedTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.SourcesUserConfiguration).Description, json);
            });
        }

        public ArticleModel GetInfoArticle()
        {
            var model = new ArticleModel()
            {
                Author = "Florian Moser",
                Title = "Info",
                SubTitle = "Informationen über diese App",
                Teaser = "OfflineMedia erlaubt das Lesen der öffentlichen Onlineausgabe von Zeitungen, auch wenn gerade kein Internet verfügbar ist.",
                DownloadDateTime = DateTime.Now,
                PublishDateTime = DateTime.Now,
                LoadingState = LoadingState.Loaded,
                PublicUri = "http://offlinemedia.florianalexandermoser.ch/"
            };
            model.Content.Add(new TextContentModel()
            {
                Content = HtmlConverter.HtmlToParagraph("<h1>Über diese App</h1>" +
                                "<p> " +
                                "Die App versucht sich bei jedem Start zu aktualisieren. Ist kein Internet vorhanden, werden die Artikel des letzten Downloads angezeigt. " +
                                "<br /><br />" +
                                "Die Zeit und das verwendete Datenvolumen, die die Aktualisierung benötigt, hängt stark von den selektierten Quellen ab. Geht die Aktualisierung zu langsam, können Sie sich überlegen, wieder einige Feeds abzuschalten. " +
                                "<br /><br />" +
                                "Die App ist gratis und wird es auch bleiben. Sie generiert keine direkten oder indirekten Einnahmen.</p>" +
                                "<h1>FAQ</h1>" +
                                "<p><b>Wie werden die Medien ausgewählt, die von dieser App unterstützt werden?</b></p>" +
                                "<p>Aufgrund der Machbarkeit einer Implementation, sowie der Anzahl wahrscheinlich interessierter Leser. " +
                                "Die Qualität der Nachrichten oder deren politische Ausrichtung ist nicht relevant, grundsätzlich wird versucht, ein möglichst breites Spektrum abzudecken. " +
                                "Medien, deren Popularität jedoch unter Anderem durch Clickbaiting (ein Beispiel für Clickbaiting: \"10 skurrile Tipps für eine erfolgreiches Leben\") gesichert wird, werden jedoch bei der Auswahl gezielt benachteiligt." +
                                "<p><b>Könntest du die Zeitung XY in die App einbinden?</b></p>" +
                                "<p>Schreibe mir eine Email an OfflineMedia@outlook.com</p>" +
                                "<p><b>Gibt es Nachrichtenportale, die wahrscheinlich nicht implementiert werden?</b></p>" +
                                "<p><b>Watson:</b> Implementierung ist zurzeit zeitaufwendig, ausserdem betreibt watson ausgiebiges Clickbaiting</p>" +
                                "<p><b>Für welche Nachrichtenportale ist eine Implementierung geplant?</b></p>" +
                                "<p>Zeit online, Süddeutsche.de, Spiegel online sowie zwei Zeitungen aus der französischen Schweiz</p>" +
                                "<h1>Über den Herausgeber</h1>" +
                                "<p>Mein Name ist Florian Moser, ich bin ein Programmierer aus Allschwil, Schweiz. <br /><br />" +
                                "Neben Apps entwickle ich auch Webseiten und Webapplikationen. Ein Kontaktformular und weitere Informationen über meine Projekte sind auf meiner Webseite zu finden.</p>" +
                                "<p><b>Webseite:</b> florianalexandermoser.ch<br />" +
                                "<b>E-Mail:</b> OfflineMedia@outlook.com</p>")
            }
                );
            return model;
        }

        public ObservableCollection<SourceModel> GetSampleSources()
        {
            var article = GetSampleArticle();
            var article2 = GetSampleArticle();
            var sm = new SourceModel()
            {
                Abbreviation = "NZZ",
                Guid = Guid.NewGuid(),
                Source = Sources.Nzz,
                Name = "Neue Zürcher Zeitung",
                LogicBaseUrl = "http://api.nzz.ch/",
                PublicBaseUrl = "http://nzz.ch/",

            };
            var fm = new FeedModel()
            {
                Name = "Home",
                Guid = Guid.NewGuid(),
                Source = sm,
                Url = "home",
                ArticleList =
                {
                    article,
                    article2,
                    article
                }
            };
            var fm2 = new FeedModel()
            {
                Name = "Ausland",
                Guid = Guid.NewGuid(),
                Source = sm,
                Url = "ausland",
                ArticleList =
                {
                    article,
                    article2,
                    article
                }
            };
            sm.ActiveFeeds.Add(fm);
            sm.AllFeeds.Add(fm);
            sm.AllFeeds.Add(fm2);
            
            return new ObservableCollection<SourceModel>()
            {
                sm,
                sm,
                sm
            };
        }

        public ArticleModel GetEmptyFeedArticle()
        {
            return GetInfoArticle();
        }


        #region Article Samples
        private ArticleModel GetSampleArticle()
        {
            var avm = new ArticleModel
            {
                Title = "Auf der Suche nach der nächsten Systemkrise",
                SubTitle = "Liquidität an den Finanzmärkten",
                Teaser = "Die Börsen sind gepannt auf die Entwicklung der strukturellen Verbesserung des isländischen Mondscheinmaterials",

                Author = "Author Maximus",
                LoadingState = LoadingState.Loaded,
                DownloadDateTime = DateTime.Now,
                PublishDateTime = DateTime.Now,
                LogicUri = "http://bazonline.ch/schweiz/standard/freie-bahn-fuer-mobility-pricing/story/24892119",
                PublicUri = "http://bazonline.ch/schweiz/standard/freie-bahn-fuer-mobility-pricing/story/24892119"
            };
            avm.Content.Add(new TextContentModel()
            {
                Content = HtmlConverter.HtmlToParagraph("<h1>Über diese App</h1>" +
                                "<p> " +
                                "Die App versucht sich bei jedem Start zu aktualisieren. Ist kein Internet vorhanden, werden die Artikel des letzten Downloads angezeigt. " +
                                "<br /><br />" +
                                "Die Zeit und das verwendete Datenvolumen, die die Aktualisierung benötigt, hängt stark von den selektierten Quellen ab. Geht die Aktualisierung zu langsam, können Sie sich überlegen, wieder einige Feeds abzuschalten. " +
                                "<br /><br />" +
                                "Die App ist gratis und wird es auch bleiben. Sie generiert keine direkten oder indirekten Einnahmen.</p>" +
                                "<h1>FAQ</h1>" +
                                "<p><b>Wie werden die Medien ausgewählt, die von dieser App unterstützt werden?</b></p>" +
                                "<p>Aufgrund der Machbarkeit einer Implementation, sowie der Anzahl wahrscheinlich interessierter Leser. " +
                                "Die Qualität der Nachrichten oder deren politische Ausrichtung ist nicht relevant, grundsätzlich wird versucht, ein möglichst breites Spektrum abzudecken. " +
                                "Medien, deren Popularität jedoch unter Anderem durch Clickbaiting (ein Beispiel für Clickbaiting: \"10 skurrile Tipps für eine erfolgreiches Leben\") gesichert wird, werden jedoch bei der Auswahl gezielt benachteiligt." +
                                "<p><b>Könntest du die Zeitung XY in die App einbinden?</b></p>" +
                                "<p>Schreibe mir eine Email an OfflineMedia@outlook.com</p>" +
                                "<p><b>Gibt es Nachrichtenportale, die wahrscheinlich nicht implementiert werden?</b></p>" +
                                "<p><b>Watson:</b> Implementierung ist zurzeit zeitaufwendig, ausserdem betreibt watson ausgiebiges Clickbaiting</p>" +
                                "<p><b>Für welche Nachrichtenportale ist eine Implementierung geplant?</b></p>" +
                                "<p>Zeit online, Süddeutsche.de, Spiegel online sowie zwei Zeitungen aus der französischen Schweiz</p>" +
                                "<h1>Über den Herausgeber</h1>" +
                                "<p>Mein Name ist Florian Moser, ich bin ein Programmierer aus Allschwil, Schweiz. <br /><br />" +
                                "Neben Apps entwickle ich auch Webseiten und Webapplikationen. Ein Kontaktformular und weitere Informationen über meine Projekte sind auf meiner Webseite zu finden.</p>" +
                                "<p><b>Webseite:</b> florianalexandermoser.ch<br />" +
                                "<b>E-Mail:</b> OfflineMedia@outlook.com</p>")
            });
            return avm;
        }
        #endregion
    }
}
