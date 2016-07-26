using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.Business.Enums;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Helpers;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Managers;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;
using Famoser.OfflineMedia.Business.Repositories.Base;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Business.Services;
using Famoser.OfflineMedia.Data.Entities.Database;
using Famoser.OfflineMedia.Data.Entities.Database.Contents;
using Famoser.OfflineMedia.Data.Entities.Database.Relations;
using Famoser.OfflineMedia.Data.Entities.Storage.Sources;
using Famoser.SqliteWrapper.Repositories;
using Famoser.SqliteWrapper.Services.Interfaces;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace Famoser.OfflineMedia.Business.Repositories
{
    public partial class ArticleRepository : BaseRepository, IArticleRepository
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IProgressService _progressService;
        private readonly IStorageService _storageService;
        private readonly ISqliteService _sqliteService;
        private readonly IPlatformCodeService _platformCodeService;
        private readonly IThemeRepository _themeRepository;
        private readonly ImageDownloadService _imageDownloadService;

        private readonly GenericRepository<ArticleModel, ArticleEntity> _articleGenericRepository;
        private readonly GenericRepository<ImageContentModel, ImageContentEntity> _imageContentGenericRepository;
        private readonly GenericRepository<TextContentModel, TextContentEntity> _textContentGenericRepository;
        private readonly GenericRepository<GalleryContentModel, GalleryContentEntity> _galleryContentGenericRepository;

#pragma warning disable 4014
        public ArticleRepository(ISettingsRepository settingsRepository, IProgressService progressService, IStorageService storageService, ISqliteService sqliteService, IPlatformCodeService platformCodeService, IThemeRepository themeRepository)
        {
            _settingsRepository = settingsRepository;
            _progressService = progressService;
            _storageService = storageService;
            _sqliteService = sqliteService;
            _platformCodeService = platformCodeService;
            _themeRepository = themeRepository;

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
                    var recovered = false;
                    try
                    {
                        var json = await _storageService.GetRoamingTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.SourcesUserConfiguration).Description);

                        if (!string.IsNullOrEmpty(json))
                        {
                            _sourceCacheEntity = JsonConvert.DeserializeObject<SourceCacheEntity>(json);
                            recovered = true;
                        }
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
                        {
                            _sourceCacheEntity.IsEnabledDictionary[source.Guid] = false;
                            recovered = false;
                        }

                        SourceManager.AddSource(source, _sourceCacheEntity.IsEnabledDictionary[source.Guid]);
                        foreach (var feedEntity in sourceEntity.Feeds)
                        {
                            if (!_sourceCacheEntity.IsEnabledDictionary.ContainsKey(feedEntity.Guid))
                            {
                                _sourceCacheEntity.IsEnabledDictionary[feedEntity.Guid] = false;
                                recovered = false;
                            }

                            var feed = EntityModelConverter.Convert(feedEntity, source, _sourceCacheEntity.IsEnabledDictionary[feedEntity.Guid]);
                            SourceManager.AddFeed(feed, source, _sourceCacheEntity.IsEnabledDictionary[feed.Guid]);

                            if (_sourceCacheEntity.IsEnabledDictionary[feed.Guid])
                                feedsToLoad.Add(feed);
                        }
                    }

                    _isInitialized = true;

                    if (!recovered)
                        await SaveCache();

                    var tasks = feedsToLoad.Select(feedModel => LoadArticlesIntoFeed(feedModel, 12)).ToList();

                    await Task.WhenAll(tasks);
                }
            });
        }

        private async Task LoadArticlesIntoFeed(FeedModel feed, int max = -1)
        {
            if (max == -1)
                max = 100;
            for (int i = 0; i < max; i++)
            {
                if (feed.AllArticles.Count <= i)
                {
                    var stringGuid = feed.Guid.ToString();
                    var realmax = max == 0 ? 0 : max - i;
                    var relations = await _sqliteService.GetByCondition<FeedArticleRelationEntity>(s => s.FeedGuid == stringGuid, s => s.Index, false, realmax, i);

                    foreach (var feedArticleRelationEntity in relations)
                    {
                        var article = await ArticleHelper.LoadForFeed(feedArticleRelationEntity.ArticleId, _sqliteService);
                        feed.AllArticles.Add(article);
                    }
                }

                _imageDownloadService.Download(feed);

                //no more entries 
                if (feed.AllArticles.Count <= i)
                    return;
            }
        }

        public Task<bool> LoadFullArticleAsync(ArticleModel am)
        {
            return ExecuteSafe(async () =>
            {
                _imageDownloadService.Download(am);

                if (am.LoadingState != LoadingState.Loading)
                {
                    if (am.Content.Any())
                        return true;

                    var id = am.GetId();
                    var contents = await _sqliteService.GetByCondition<ContentEntity>(s => s.ParentId == id, s => s.Index, false, 0, 0);
                    for (int index = 0; index < contents.Count; index++)
                    {
                        if (am.Content.Count > index && am.Content[index].GetId() == contents[index].ContentId)
                            continue;

                        var contentEntity = contents[index];
                        switch (contentEntity.ContentType)
                        {
                            case (int) ContentType.Text:
                            {
                                var text = await _textContentGenericRepository.GetByIdAsync(contentEntity.ContentId);
                                text.Content = text.ContentJson != null
                                    ? JsonConvert.DeserializeObject<ObservableCollection<ParagraphModel>>(
                                        text.ContentJson)
                                    : new ObservableCollection<ParagraphModel>();
                                am.Content.Add(text);
                                break;
                            }
                            case (int) ContentType.Image:
                            {
                                var image = await _imageContentGenericRepository.GetByIdAsync(contentEntity.ContentId);
                                am.Content.Add(image);
                                break;
                            }
                            case (int) ContentType.Gallery:
                            {
                                var amId = am.GetId();
                                var galleryContents =
                                    await
                                        _sqliteService.GetByCondition<ContentEntity>(s => s.ParentId == amId,
                                            s => s.Index, false, 0, 0);
                                var gallery =
                                    await _galleryContentGenericRepository.GetByIdAsync(contentEntity.ContentId);
                                am.Content.Add(gallery);

                                foreach (
                                    var galleryContent in
                                        galleryContents.Where(g => g.ContentType == (int) ContentType.Image))
                                {
                                    var image =
                                        await _imageContentGenericRepository.GetByIdAsync(galleryContent.ContentId);
                                    gallery.Images.Add(image);
                                }
                                break;
                            }
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
                await LoadArticlesIntoFeed(fm);

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

        public Task<bool> SwitchFeedActiveStateAsync(FeedModel feedModel)
        {
            return SetFeedActiveStateAsync(feedModel, !SourceManager.GetFeedActiveState(feedModel));
        }

        public Task<bool> SwitchSourceActiveStateAsync(SourceModel sourceModel)
        {
            return SetSourceActiveStateAsync(sourceModel, !SourceManager.GetSourceActiveState(sourceModel));
        }

        private Task<bool> SaveCache()
        {
            return ExecuteSafe(async () =>
            {
                var json = JsonConvert.SerializeObject(_sourceCacheEntity);
                return await _storageService.SetRoamingTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.SourcesUserConfiguration).Description, json);
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
                Content = HtmlConverter.CreateOnce().HtmlToParagraph("<h1>Über diese App</h1>" +
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
    }
}
