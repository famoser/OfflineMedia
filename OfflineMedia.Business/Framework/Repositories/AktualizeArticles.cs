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
using OfflineMedia.Data;
using OfflineMedia.Data.Entities;
using SQLite.Net.Attributes;

namespace OfflineMedia.Business.Framework.Repositories
{
    public partial class ArticleRepository
    {
        private const int ConcurrentThreads = 4;
        private readonly List<Task> _aktualizeArticlesTasks = new List<Task>();
        private readonly List<ArticleModel> _newArticleModels = new List<ArticleModel>();
        private readonly List<ArticleModel> _toDatabaseArticles = new List<ArticleModel>();
        private readonly List<ArticleModel> _toDatabaseFlatArticles = new List<ArticleModel>();
        private int _newArticles;

        private readonly List<Task> _actualizeFeedsTasks = new List<Task>();
        private readonly List<List<ArticleModel>> _toDatabaseFeeds = new List<List<ArticleModel>>();
        private int _newFeeds;

        public bool ActualizeArticle(ArticleModel article)
        {
            if (_newArticleModels.Contains(article))
            {
                _newArticleModels.Remove(article);
            }
            _newArticleModels.Insert(0, article);

            if (!_aktualizeArticlesTasks.Any())
            {
                var tsk = AktualizeArticlesTask(false);
                _aktualizeArticlesTasks.Add(tsk);
            }
            return true;
        }

        private List<FeedModel> _newFeedModels;
        public async Task ActualizeArticles()
        {
            try
            {
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

                using (var unitOfWork = new UnitOfWork(true))
                {
                    var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                    var state = (int)ArticleState.New;
                    var newarticles = await repo.GetByCondition(d => d.State == state);
                    foreach (var articleModel in newarticles)
                    {
                        if (_newArticleModels.All(d => d.Id != articleModel.Id))
                        {
                            _newArticleModels.Add(await AddModels(articleModel, await unitOfWork.GetDataService(), true));
                        }
                    }
                }

                _newArticles = _newArticleModels.Count;

                if (_newArticles > 0)
                {
                    //Actualize articles
                    if (_aktualizeArticlesTasks.Count < ConcurrentThreads)
                    {
                        for (int i = 0; i < ConcurrentThreads; i++)
                        {
                            var tsk = AktualizeArticlesTask();
                            _aktualizeArticlesTasks.Add(tsk);
                        }
                    }

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

                _progressService.HideProgress();
                _progressService.ShowDecentInformationMessage("Aktualisierung abgeschlossen", TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ActualizeArticle failed", ex);
                _progressService.HideProgress();
                _progressService.ShowDecentInformationMessage("Aktualisierung fehlgeschlagen", TimeSpan.FromSeconds(3));
            }
        }

        private async Task AktualizeFeedsTask()
        {
            while (_newFeedModels.Any())
            {
                var feed = _newFeedModels[0];
                _newFeedModels.Remove(feed);

                var percentage = Convert.ToInt32((_newFeeds - _newFeedModels.Count) * 100 / _newFeeds);
                _progressService.ShowProgress("Feed wird heruntergeladen...", percentage);

                var newfeed = await FeedHelper.Instance.DownloadFeed(feed);

                if (newfeed != null)
                {
                    ArticleHelper.Instance.AddWordDumpFromFeed(ref newfeed);

                    if (percentage == 100)
                    {
                        _progressService.ShowIndeterminateProgress(IndeterminateProgressKey.FeedSaveToDatabase);
                        _progressService.HideProgress();
                    }
                    _toDatabaseFeeds.Add(newfeed);
                    await FeedToDatabase();

                    if (percentage == 100)
                    {
                        _progressService.HideIndeterminateProgress(IndeterminateProgressKey.FeedSaveToDatabase);
                    }
                }
            }
        }

        private async Task AktualizeArticlesTask(bool showprogress = true)
        {
            using (var unitOfWork = new UnitOfWork(false))
            {
                while (_newArticleModels.Any())
                {
                    var article = _newArticleModels.OrderByDescending(d => d.PublicationTime).FirstOrDefault();
                    _newArticleModels.Remove(article);

                    article.State = ArticleState.Loading;
                    _toDatabaseFlatArticles.Add(article);
                    await ToDatabase();

                    //Messenger.Default.Send(article, Messages.ArticleRefresh);

                    //this comes from the cache, so its OK to use here
                    article.FeedConfiguration = await _settingsRepository.GetFeedConfigurationFor(article.FeedConfigurationId, await unitOfWork.GetDataService());

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
                        var percentage = Convert.ToInt32(100 - ((_newArticleModels.Count * 100) / _newArticles));
                        _progressService.ShowProgress("Artikel werden heruntergeladen...", percentage);
                    }
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

                while (_toDatabaseArticles.Any() || _toDatabaseFlatArticles.Any())
                {
                    var flatarticle = _toDatabaseFlatArticles.FirstOrDefault();
                    if (flatarticle != null)
                    {
                        _toDatabaseFlatArticles.Remove(flatarticle);
                        await UpdateArticleFlat(flatarticle, await unitOfWork.GetDataService());
                        Messenger.Default.Send(flatarticle, Messages.ArticleRefresh);
                    }
                    else
                    {
                        var complexarticles = _toDatabaseArticles.FirstOrDefault();
                        if (complexarticles != null)
                        {
                            _toDatabaseArticles.Remove(complexarticles);
                            await InsertOrUpdateArticleAndTraces(complexarticles, await unitOfWork.GetDataService());
                            Messenger.Default.Send(complexarticles, Messages.ArticleRefresh);
                        }
                    }
                }
            }
            _toDatabaseIsActive = false;
        }

        private bool _feedToDatabaseRunning;
        private async Task FeedToDatabase()
        {
            if (_feedToDatabaseRunning)
                return;

            _feedToDatabaseRunning = true;
            var deleteArticles = new List<int>();
            var newArticles = new List<ArticleModel>();
            var executes = new List<Action>();
            try
            {
                using (var unitOfWork = new UnitOfWork(false))
                {
                    while (_toDatabaseFeeds.Any())
                    {
                        var articles = _toDatabaseFeeds[0];
                        _toDatabaseFeeds.Remove(articles);
                        if (articles.Any())
                        {
                            var repo =
                                new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                            var guidString = articles.FirstOrDefault().FeedConfiguration.Guid.ToString();
                            var oldfeed = await repo.GetByCondition(a => a.FeedConfigurationId == guidString);
                            for (int index = 0; index < articles.Count; index++)
                            {
                                var articleModel = articles[index];
                                var oldmodel = oldfeed.FirstOrDefault(d => d.PublicUri == articleModel.PublicUri);
                                if (oldmodel != null)
                                {
                                    articles.Remove(articleModel);
                                    oldfeed.Remove(oldmodel);
                                    index--;
                                }
                            }

                            //delete old ones
                            deleteArticles.AddRange(oldfeed.Where(d => !d.IsFavorite).Select(d => d.Id).ToList());

                            //only new ones left
                            newArticles.AddRange(articles);

                            _newArticleModels.AddRange(articles);
                            executes.Add(() => Messenger.Default.Send(articles, Messages.FeedRefresh));
                        }
                    }

                    await DeleteAllArticlesAndTrances(deleteArticles, await unitOfWork.GetDataService());
                    await InsertAllArticleAndTraces(newArticles, await unitOfWork.GetDataService());

                    foreach (var execute in executes)
                    {
                        execute.Invoke();
                    }

                    await unitOfWork.Commit();
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
