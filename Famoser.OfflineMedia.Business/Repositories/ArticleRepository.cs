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
using Famoser.OfflineMedia.Business.Services.Interfaces;
using Famoser.OfflineMedia.Data.Entities.Database;
using Famoser.OfflineMedia.Data.Entities.Database.Contents;
using Famoser.OfflineMedia.Data.Entities.Database.Relations;
using Famoser.OfflineMedia.Data.Entities.Storage.Sources;
using Famoser.SqliteWrapper.Repositories;
using Famoser.SqliteWrapper.Services.Interfaces;
using Newtonsoft.Json;
using Nito.AsyncEx;
using IProgressService = Famoser.OfflineMedia.Business.Services.Interfaces.IProgressService;

namespace Famoser.OfflineMedia.Business.Repositories
{
    public partial class ArticleRepository : BaseRepository, IArticleRepository
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IProgressService _progressService;
        private readonly IStorageService _storageService;
        private readonly ISqliteService _sqliteService;
        private readonly IThemeRepository _themeRepository;
        private readonly IImageDownloadService _imageDownloadService;
        private readonly IPermissionsService _permissionsService;

        private readonly GenericRepository<ArticleModel, ArticleEntity> _articleGenericRepository;
        private readonly GenericRepository<ImageContentModel, ImageContentEntity> _imageContentGenericRepository;
        private readonly GenericRepository<TextContentModel, TextContentEntity> _textContentGenericRepository;
        private readonly GenericRepository<GalleryContentModel, GalleryContentEntity> _galleryContentGenericRepository;

#pragma warning disable 4014
        public ArticleRepository(ISettingsRepository settingsRepository, IProgressService progressService, IStorageService storageService, ISqliteService sqliteService, IThemeRepository themeRepository, IImageDownloadService imageDownloadService, IPermissionsService permissionsService)
        {
            _settingsRepository = settingsRepository;
            _progressService = progressService;
            _storageService = storageService;
            _sqliteService = sqliteService;
            _themeRepository = themeRepository;
            _imageDownloadService = imageDownloadService;
            _permissionsService = permissionsService;

            _articleGenericRepository = new GenericRepository<ArticleModel, ArticleEntity>(_sqliteService);
            _imageContentGenericRepository = new GenericRepository<ImageContentModel, ImageContentEntity>(_sqliteService);
            _textContentGenericRepository = new GenericRepository<TextContentModel, TextContentEntity>(_sqliteService);
            _galleryContentGenericRepository = new GenericRepository<GalleryContentModel, GalleryContentEntity>(_sqliteService);

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
                    var feeds = JsonConvert.DeserializeObject<List<SourceModel>>(jsonAssets);
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
                    foreach (var source in feeds)
                    {
                        if (!_sourceCacheEntity.IsEnabledDictionary.ContainsKey(source.Guid))
                        {
                            _sourceCacheEntity.IsEnabledDictionary[source.Guid] = false;
                            recovered = false;
                        }

                        SourceManager.AddSource(source, _sourceCacheEntity.IsEnabledDictionary[source.Guid]);
                        foreach (var feed in source.Feeds)
                        {
                            feed.Source = source;

                            if (!_sourceCacheEntity.IsEnabledDictionary.ContainsKey(feed.Guid))
                            {
                                _sourceCacheEntity.IsEnabledDictionary[feed.Guid] = false;
                                recovered = false;
                            }

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
                        var article = await LoadHelper.LoadForFeed(feedArticleRelationEntity.ArticleId, feed, _sqliteService, _imageDownloadService);
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

                if (am.LoadingState == LoadingState.Loaded)
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
                            case (int)ContentType.Text:
                                {
                                    var text = await _textContentGenericRepository.GetByIdAsync(contentEntity.ContentId);
                                    text.Content = text.ContentJson != null
                                        ? JsonConvert.DeserializeObject<ObservableCollection<ParagraphModel>>(
                                            text.ContentJson)
                                        : new ObservableCollection<ParagraphModel>();
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
                                            galleryContents.Where(g => g.ContentType == (int)ContentType.Image))
                                    {
                                        var image =
                                            await _imageContentGenericRepository.GetByIdAsync(galleryContent.ContentId);
                                        gallery.Images.Add(image);
                                    }
                                    break;
                                }
                        }
                    }

                    _themeRepository.LoadArticleThemesAsync(am);
                }
                else if (am.LoadingState == LoadingState.New)
                {
                    if (await _permissionsService.CanDownloadArticles())
                        await ActualizeArticleAsync(am);
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
                Content = HtmlConverter.CreateOnce("").HtmlToParagraph("<h2>Was ist diese App?</h2>" +
                                "<p>Mit dieser App können Sie die meisten Nachrichtenportale der Schweiz, sowie einige aus Deutschland lesen. " +
                                "Die App ist gratis und wird es auch bleiben. Sie generiert keine direkten oder indirekten Einnahmen.</p>" +
                                "<h2>Wie werden die Medien ausgewählt, die von dieser App unterstützt werden?</h2>" +
                                "<p>Aufgrund der Machbarkeit einer Implementation, sowie der Anzahl wahrscheinlich interessierter Leser. " +
                                "Die Qualität der Nachrichten oder deren politische Ausrichtung ist nicht relevant, grundsätzlich versuche ich, möglicht alle wichtigen Nachrichtenkanäle zu integrieren. " +
                                "Medien, deren Popularität jedoch vor allem durch Clickbaiting (ein Beispiel für Clickbaiting: \"10 skurrile Tipps für eine erfolgreiches Leben\") gesichert wird, werde ich nicht implementieren." +
                                "Kontaktieren Sie mich, falls Sie eine Zeitung integriert haben möchten. </p>" +
                                "<h2>Gibt es Nachrichtenportale, die wahrscheinlich nicht implementiert werden?</h2>" +
                                "<p><b>Watson:</b> Die Implementierung ist zeitaufwendig, zudem betreibt watson ausgiebiges Clickbaiting</p>" +
                                "<h2>Für welche Nachrichtenportale ist eine Implementierung geplant?</h2>" +
                                "<p>Süddeutsche.de ist geplant, konnte ich bis jetzt aber nicht umsetzen</p>"
                 )
            });
            return model;
        }
    }
}
