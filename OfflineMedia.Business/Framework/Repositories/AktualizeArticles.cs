using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources;
using OfflineMedia.Common.Enums.View;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Timer;
using OfflineMedia.Data;
using OfflineMedia.Data.Entities;
using SQLite.Net.Attributes;

namespace OfflineMedia.Business.Framework.Repositories
{
    public partial class ArticleRepository
    {
        private const int ConcurrentThreads = 1;
        private readonly List<Task> _aktualizeArticlesTasks = new List<Task>();
        private List<ArticleModel> _aktualizeArticleModels = new List<ArticleModel>();
        private readonly List<ArticleModel> _toDatabaseArticles = new List<ArticleModel>();
        private readonly List<ArticleModel> _toDatabaseFlatArticles = new List<ArticleModel>();
        private int _newArticles;

        private readonly List<Task> _actualizeFeedsTasks = new List<Task>();
        private readonly List<List<ArticleModel>> _toDatabaseFeeds = new List<List<ArticleModel>>();
        private int _newFeeds;

        public async Task<bool> ActualizeArticle(ArticleModel article)
        {
            if (_aktualizeArticleModels.Contains(article))
                _aktualizeArticleModels.Remove(article);

            _aktualizeArticleModels.Insert(0, article);

            if (!_aktualizeArticlesTasks.Any())
            {
                var tsk = AktualizeArticlesTask(false);
                _aktualizeArticlesTasks.Add(tsk);
            }
            return true;
        }

        private List<FeedModel> _newFeedModels;
        private bool _actualizeActive;
        private bool _actualizeRequested;
        public async Task ActualizeArticles()
        {
            try
            {
                TimerHelper.Instance.Stop("Inside Method...", this);
                if (_actualizeActive)
                {
                    _actualizeRequested = true;
                    return;
                }
                _actualizeActive = true;

                TimerHelper.Instance.Stop("Creating Feed Tasks", this);
                //Get Total Number of feeds
                _newFeeds = _sources.SelectMany(source => source.FeedList).Count();

                _newFeedModels = new List<FeedModel>();
                foreach (var sourceModel in _sources)
                {
                    if (sourceModel.SourceConfiguration.Source == SourceEnum.Favorites)
                        continue;

                    foreach (var feed in sourceModel.FeedList)
                    {
                        _newFeedModels.Add(feed);
                    }
                }


                //Actualize feeds
                if (_actualizeFeedsTasks.Count < ConcurrentThreads)
                {
                    for (int i = 0; i < ConcurrentThreads; i++)
                    {
                        var tsk = AktualizeFeedsTask();
                        _actualizeFeedsTasks.Add(tsk);
                    }
                }

                TimerHelper.Instance.Stop("Waiting for Feed Tasks", this);
                while (_actualizeFeedsTasks.Count > 0)
                {
                    try
                    {
                        var tsk = _actualizeFeedsTasks.FirstOrDefault();
                        if (tsk != null)
                        {
                            if (tsk.Status <= TaskStatus.Running)
                                await tsk;
                            _actualizeFeedsTasks.Remove(tsk);
                        }
                    }
                    //may raise exception because list is emptied in excatly that moment
                    catch (Exception ex)
                    {
                        LogHelper.Instance.Log(LogLevel.Error, this, "Exception occured while waiting for tasks to complete (1)", ex);
                    }
                }
                TimerHelper.Instance.Stop("Saving forced to database", this);
                _progressService.ShowIndeterminateProgress(IndeterminateProgressKey.FeedSaveToDatabase);
                await FeedToDatabase(true);
                _progressService.HideIndeterminateProgress(IndeterminateProgressKey.FeedSaveToDatabase);

                TimerHelper.Instance.Stop("Get additional articles from Database", this);
                _aktualizeArticleModels = _aktualizeArticleModels.Where(a => a.State == ArticleState.New).ToList();
                using (var unitOfWork = new UnitOfWork(true))
                {
                    foreach (var articleModel in _aktualizeArticleModels)
                    {
                        if (_aktualizeArticleModels.All(d => d.Id != articleModel.Id))
                        {
                            _aktualizeArticleModels.Add(await AddModels(articleModel, await unitOfWork.GetDataService()));
                        }
                    }
                }

                _newArticles = _aktualizeArticleModels.Count;

                if (_newArticles > 0)
                {
                    TimerHelper.Instance.Stop("Actualizing articles", this);
                    //Actualize articles
                    if (_aktualizeArticlesTasks.Count < ConcurrentThreads)
                    {
                        for (int i = 0; i < ConcurrentThreads; i++)
                        {
                            var tsk = AktualizeArticlesTask();
                            _aktualizeArticlesTasks.Add(tsk);
                        }
                    }

                    TimerHelper.Instance.Stop("Waiting for Tasks", this);
                    while (_aktualizeArticlesTasks.Count > 0)
                    {
                        try
                        {
                            var tsk = _aktualizeArticlesTasks.FirstOrDefault();
                            if (tsk != null)
                            {
                                if (tsk.Status <= TaskStatus.Running)
                                    await tsk;
                                _aktualizeArticlesTasks.Remove(tsk);
                            }
                        }
                        //may raise exception because list is emptied in excatly that moment
                        catch (Exception ex)
                        {
                            LogHelper.Instance.Log(LogLevel.Error, this,
                                "Exception occured while waiting for tasks to complete (2)", ex);
                        }
                    }
                }
                TimerHelper.Instance.Stop("Finished", this);

                _progressService.HideProgress();
                _progressService.ShowDecentInformationMessage("Aktualisierung abgeschlossen", TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ActualizeArticle failed", ex);
                _progressService.HideProgress();
                _progressService.ShowDecentInformationMessage("Aktualisierung fehlgeschlagen", TimeSpan.FromSeconds(3));
            }
            _actualizeActive = false;
            if (_actualizeRequested)
            {
                _actualizeRequested = false;
                await ActualizeArticles();
            }
        }

        private async Task AktualizeFeedsTask()
        {
            var guid = Guid.NewGuid();
            TimerHelper.Instance.Stop("Aktualize Feed Task started...", this, guid);
            while (_newFeedModels.Any())
            {
                TimerHelper.Instance.Stop("Downloading Feed...", this, guid);
                var feed = _newFeedModels[0];
                _newFeedModels.Remove(feed);

                var newfeed = await FeedHelper.Instance.DownloadFeed(feed);

                var percentage = Convert.ToInt32((_newFeeds - _newFeedModels.Count) * 100 / _newFeeds * 2);
                _progressService.ShowProgress("Feed wird heruntergeladen...", percentage);

                TimerHelper.Instance.Stop("Downloaded Feed, Inserting to Database", this, guid);
                if (newfeed != null)
                {
                    _toDatabaseFeeds.Add(newfeed);
                    await FeedToDatabase();
                }
                TimerHelper.Instance.Stop("Finished, proceeding to next item", this, guid);
            }
            TimerHelper.Instance.Stop("No more models, terminating", this, guid);
        }

        private async Task AktualizeArticlesTask(bool showprogress = true)
        {
            while (_aktualizeArticleModels.Any())
            {
                var article = _aktualizeArticleModels.OrderByDescending(d => d.PublicationTime).FirstOrDefault();
                _aktualizeArticleModels.Remove(article);

                article.State = ArticleState.Loading;
                _toDatabaseFlatArticles.Add(article);
                await ToDatabase();

                //Messenger.Default.Send(article, Messages.ArticleRefresh);

                //this comes from the cache, so its OK to use here
                article.FeedConfiguration = await _settingsRepository.GetFeedConfigurationFor(article.FeedConfigurationId);

                IMediaSourceHelper sh = ArticleHelper.Instance.GetMediaSource(article);
                if (sh == null)
                {
                    LogHelper.Instance.Log(LogLevel.Warning, this,
                        "ArticleHelper.DownloadArticle: Tried to Download Article which cannot be downloaded");
                    article.State = ArticleState.WrongSourceFaillure;
                    _toDatabaseFlatArticles.Add(article);
                    await ToDatabase();
                }
                else
                {
                    article = await ActualizeArticle(article, sh);

                    _toDatabaseArticles.Add(article);
                    await ToDatabase();
                }

                if (showprogress)
                {
                    var percentage = Convert.ToInt32(100 - ((_aktualizeArticleModels.Count * 100) / _newArticles));
                    _progressService.ShowProgress("Artikel werden heruntergeladen...", percentage);
                }
            }
        }

        private bool _toDatabaseIsActive = false;
        private async Task ToDatabase()
        {
            if (_toDatabaseIsActive)
                return;
            _toDatabaseIsActive = true;
            using (var unitOfWork = new UnitOfWork(false))
            {
                await _themeRepository.SaveChanges(await unitOfWork.GetDataService());

                if (_toDatabaseFlatArticles.Any())
                {
                    var list = new List<ArticleModel>(_toDatabaseFlatArticles);
                    _toDatabaseFlatArticles.Clear();
                    var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                    await repo.UpdateAll(list);
                }

                while (_toDatabaseArticles.Any())
                {
                    var complexarticle = _toDatabaseArticles.FirstOrDefault();
                    if (complexarticle != null)
                    {
                        _toDatabaseArticles.Remove(complexarticle);
                        await InsertOrUpdateArticleAndTraces(complexarticle, await unitOfWork.GetDataService());
                    }
                }
            }
            _toDatabaseIsActive = false;
        }

        private bool _feedToDatabaseRunning;
        private readonly List<int> _deleteArticlesFromDatabase = new List<int>();
        private readonly List<ArticleModel> _newArticlesToDatabase = new List<ArticleModel>();
        private async Task FeedToDatabase(bool force = false)
        {
            if (_feedToDatabaseRunning)
                return;

            _feedToDatabaseRunning = true;
            try
            {
                var work = _newArticlesToDatabase.Count + _deleteArticlesFromDatabase.Count;
                var g = Guid.NewGuid();
                TimerHelper.Instance.Stop("Sorting articles...", this, g);
                while (_toDatabaseFeeds.Any())
                {
                    var articles = _toDatabaseFeeds[0];
                    _toDatabaseFeeds.Remove(articles);
                    if (articles.Any())
                    {
                        var oldDatabaseArticles = _repoArticles.Where(a => a.FeedConfigurationId == articles.FirstOrDefault().FeedConfiguration.Guid).ToList();

                        var tosend = new List<ArticleModel>();
                        var newarticles = new List<ArticleModel>();
                        foreach (var article in articles)
                        {
                            var oldDatabaseModel = oldDatabaseArticles.FirstOrDefault(d => d.PublicUri == article.PublicUri);
                            if (oldDatabaseModel == null)
                            {
                                _repoArticles.Add(article);
                                newarticles.Add(article);
                                tosend.Add(article);
                            }
                            else
                            {
                                oldDatabaseArticles.Remove(oldDatabaseModel);
                                tosend.Add(oldDatabaseModel);
                            }
                        }


                        //if (newarticles.Any())
                        //    await FeedHelper.Instance.DownloadPictures(newarticles);

                        Messenger.Default.Send(tosend, Messages.FeedRefresh);

                        //delete old ones
                        _deleteArticlesFromDatabase.AddRange(oldDatabaseArticles.Where(d => !d.IsFavorite).Select(d => d.Id).ToList());

                        //only new ones left
                        _newArticlesToDatabase.AddRange(newarticles);
                    }
                }
                TimerHelper.Instance.Stop("Sorting finished...", this, g);

                if (work > 100 || force)
                {
                    using (var unitOfWork = new UnitOfWork(false))
                    {
                        TimerHelper.Instance.Stop("Deleting articles...", this, g);
                        await DeleteAllArticlesAndTrances(_deleteArticlesFromDatabase, await unitOfWork.GetDataService());
                        _deleteArticlesFromDatabase.Clear();

                        TimerHelper.Instance.Stop("InsertAllArticleAndTraces...", this, g);
                        await InsertAllArticleAndTraces(_newArticlesToDatabase, await unitOfWork.GetDataService());
                        _aktualizeArticleModels.AddRange(_newArticlesToDatabase);
                        _newArticlesToDatabase.Clear();

                        TimerHelper.Instance.Stop("Commiting Changes...", this, g);
                        await unitOfWork.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Exception occured", ex);
            }
            _feedToDatabaseRunning = false;
        }

        private async Task DeleteAllArticlesAndTrances(List<int> newarticlesId, IDataService dataService)
        {
            try
            {
                await dataService.DeleteArticlesById(newarticlesId);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Article cannot be deleted", ex);
            }
        }

        private async Task InsertOrUpdateArticleAndTraces(ArticleModel article, IDataService dataService)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(false))
                {
                    article.PrepareForSave();

                    var addimages = new List<ImageModel>();
                    var updateimages = new List<ImageModel>();
                    //article

                    var addgalleries = new List<GalleryModel>();
                    var updategalleries = new List<GalleryModel>();
                    var addgalleryImages = new List<ImageModel>();
                    var updategalleryImages = new List<ImageModel>();
                    var addcontents = new List<ContentModel>();
                    var updatecontents = new List<ContentModel>();


                    var articleRepo = new GenericRepository<ArticleModel, ArticleEntity>(dataService);
                    var imageRepo = new GenericRepository<ImageModel, ImageEntity>(dataService);
                    var galleryRepo = new GenericRepository<GalleryModel, GalleryEntity>(dataService);
                    var contentRepo = new GenericRepository<ContentModel, ContentEntity>(dataService);

                    if (article.LeadImage != null)
                    {
                        if (article.LeadImage.Id == 0)
                            addimages.Add(article.LeadImage);
                        else
                            updateimages.Add(article.LeadImage);
                    }
                    if (article.Content != null && article.Content.Any())
                    {
                        foreach (var contentModel in article.Content)
                        {
                            if (contentModel.Id == 0)
                                addcontents.Add(contentModel);
                            else
                                updatecontents.Add(contentModel);

                            if (contentModel.ContentType == ContentType.Gallery && contentModel.Gallery != null)
                            {
                                if (contentModel.Gallery.Id == 0)
                                    addgalleries.Add(contentModel.Gallery);
                                else
                                    updategalleries.Add(contentModel.Gallery);

                                if (contentModel.Gallery.Images != null)
                                {
                                    foreach (var imageModel in contentModel.Gallery.Images)
                                    {
                                        if (imageModel.Id == 0)
                                            addgalleryImages.Add(imageModel);
                                        else
                                            updategalleryImages.Add(imageModel);
                                    }
                                    addgalleryImages.AddRange(contentModel.Gallery.Images);
                                }
                            }
                            else if (contentModel.ContentType == ContentType.Image && contentModel.Image != null)
                            {
                                if (contentModel.Image.Id == 0)
                                    addimages.Add(contentModel.Image);
                                else
                                    updateimages.Add(article.LeadImage);
                            }
                        }
                    }

                    if (article.Themes != null)
                        await _themeRepository.SetThemesByArticle(article.Id, article.Themes.Select(t => t.Id).ToList());

                    if (article.RelatedArticles != null)
                        await SetRelatedArticlesByArticleId(article.Id, article.RelatedArticles.Select(t => t.Id).ToList(), dataService);

                    await imageRepo.AddAll(addimages);
                    await galleryRepo.AddAll(addgalleries);
                    await imageRepo.AddAll(addgalleryImages);
                    await contentRepo.AddAll(addcontents);

                    if (article.Id == 0)
                        await articleRepo.Add(article);
                    else
                        await articleRepo.Update(article);

                    await imageRepo.UpdateAll(updateimages);
                    await galleryRepo.UpdateAll(updategalleries);
                    await imageRepo.UpdateAll(updategalleryImages);
                    await contentRepo.UpdateAll(updatecontents);

                    await _themeRepository.SaveChanges();

                    await unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Article cannot be saved", ex);
            }
        }

        private async Task<ArticleModel> ActualizeArticle(ArticleModel article, IMediaSourceHelper sh)
        {
            try
            {
                if (sh.NeedsToEvaluateArticle())
                {
                    if (article.LeadImage?.Url != null && !article.LeadImage.IsLoaded)
                    {
                        article.LeadImage.Image = await Download.DownloadImageAsync(article.LeadImage.Url);
                        article.LeadImage.IsLoaded = true;
                    }

                    string articleresult = await Download.DownloadStringAsync(article.LogicUri);
                    var tuple = await sh.EvaluateArticle(articleresult, article);
                    if (tuple.Item1)
                    {
                        if (sh.WriteProperties(ref article, tuple.Item2))
                        {
                            article.WordDump = string.Join(" ", sh.GetKeywords(article));
                            article.State = ArticleState.Loaded;
                            ArticleHelper.Instance.OptimizeArticle(ref article);
                        }
                        else
                        {
                            article.State = ArticleState.WritePropertiesFaillure;
                        }
                    }
                    else
                    {
                        article.State = ArticleState.EvaluateArticleFaillure;
                    }
                }
                else
                {
                    article.State = ArticleState.Loaded;
                    ArticleHelper.Instance.OptimizeArticle(ref article);
                }

                ArticleHelper.Instance.AddWordDumpFromArticle2(ref article);
                return article;
            }
            catch (Exception ex)
            {
                if (article != null)
                    LogHelper.Instance.Log(LogLevel.Error, this, "ActualizeArticle failed! Source: " + article.SourceConfigurationId + " URL: " + article.PublicUri, ex);
            }
            return article;
        }
    }
}
