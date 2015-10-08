using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Enums.Models;
using OfflineMediaV3.Business.Framework.Generic;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Helpers;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Models.NewsModel.NMModels;
using OfflineMediaV3.Business.Sources;
using OfflineMediaV3.Business.Sources.Blick;
using OfflineMediaV3.Business.Sources.Nzz;
using OfflineMediaV3.Business.Sources.Postillon;
using OfflineMediaV3.Business.Sources.Tamedia;
using OfflineMediaV3.Business.Sources.ZwanzigMin;
using OfflineMediaV3.Common.DebugHelpers;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.Data;
using OfflineMediaV3.Data.Entities;

namespace OfflineMediaV3.Business.Framework.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private ISettingsRepository _settingsRepository;
        private IThemeRepository _themeRepository;
        private IProgressService _progressService;

        public ArticleRepository(ISettingsRepository settingsRepository, IThemeRepository themeRepository, IProgressService progressService)
        {
            _settingsRepository = settingsRepository;
            _themeRepository = themeRepository;
            _progressService = progressService;
        }

        public ObservableCollection<SourceModel> GetSampleArticles()
        {
            var oc = new ObservableCollection<SourceModel>();
            for (int i = 0; i < 3; i++)
            {
                var sourceModel = new SourceModel()
                {
                    SourceConfiguration = new SourceConfigurationModel()
                    {
                        BoolValue = true,
                        SourceNameLong = "Long Source Name",
                        SourceNameShort = "Short Source Name"
                    },
                    FeedList = new ObservableCollection<FeedModel>()
                };
                for (int j = 0; j < 3; j++)
                {
                    var feedModel = new FeedModel()
                    {
                        ArticleList = new ObservableCollection<ArticleModel>()
                        {
                            GetSampleArticle(),
                            GetSampleArticle(),
                            GetSampleArticle(),
                            GetSampleArticle(),
                            GetSampleArticle(),
                        },
                        Source = sourceModel,
                        FeedConfiguration = new FeedConfigurationModel()
                        {
                            Name = "Feed",
                            BoolValue = true
                        },
                    };
                    sourceModel.FeedList.Add(feedModel);
                }
                oc.Add(sourceModel);
            }
            return oc;
        }

        public ArticleModel GetInfoArticle()
        {
            return new ArticleModel()
            {
                Author = "Florian Moser",
                Content = new List<ContentModel>
                {
                    new ContentModel()
                    {
                        ContentType = ContentType.Html,
                        Html = "<h1>Über diese App</h1>" +
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
                                "<b>E-Mail:</b> OfflineMedia@outlook.com</p>"
                    }
                },
                Title = "Info",
                SubTitle = "Informationen über diese App",
                Teaser = "OfflineMedia erlaubt das Lesen der öffentlichen Onlineausgabe von Zeitungen, auch wenn gerade kein Internet verfügbar ist.",
                Themes = new List<ThemeModel>() { new ThemeModel() { Name = "Info" } },
                ChangeDate = DateTime.Now,
                CreateDate = DateTime.Now,
                IsStatic = true,
                PublicationTime = DateTime.Now,
                State = ArticleState.Loaded,
                PublicUri = new Uri("http://offlinemedia.florianalexandermoser.ch/")
            };
        }

        public ArticleModel GetEmptyFeedArticle()
        {
            return new ArticleModel()
            {
                Author = "Florian Moser",
                Content = new List<ContentModel>
                {
                    new ContentModel()
                    {
                        ContentType = ContentType.Html,
                        Html = "<h1>Dieser Feed wurde noch nicht heruntergeladen</h1>"
                    }
                },
                Title = "Feed noch leer",
                SubTitle = "Dieser Feed wurde noch nicht heruntergeladen",
                Teaser = "Verbinden Sie sich mit dem Internet, um diesen Feed zu aktualisieren",
                Themes = new List<ThemeModel>() { new ThemeModel() { Name = "Info" } },
                ChangeDate = DateTime.Now,
                CreateDate = DateTime.Now,
                IsStatic = true,
                PublicationTime = DateTime.Now,
                State = ArticleState.Loaded,
                PublicUri = new Uri("http://offlinemedia.florianalexandermoser.ch/")
            };
        }

        public async Task UpdateArticle(ArticleModel article)
        {
            using (var unitOfWork = new UnitOfWork(false))
            {
                await InsertOrUpdateArticleAndTraces(article, await unitOfWork.GetDataService());
            }
        }

        private ObservableCollection<SourceModel> _sources;
        public async Task<ObservableCollection<SourceModel>> GetSources()
        {
            _sources = new ObservableCollection<SourceModel>();
            using (var unitOfWork = new UnitOfWork(true))
            {
                var sources = await _settingsRepository.GetSourceConfigurations(await unitOfWork.GetDataService());

                foreach (var source in sources)
                {
                    if (source.BoolValue)
                    {
                        var sourceModel = new SourceModel()
                        {
                            SourceConfiguration = source,
                            FeedList = new ObservableCollection<FeedModel>(),
                        };
                        foreach (var feedConfigurationModel in source.FeedConfigurationModels)
                        {
                            if (feedConfigurationModel.BoolValue)
                            {
                                var feedModel = new FeedModel()
                                {
                                    FeedConfiguration = feedConfigurationModel,
                                    ArticleList = new ObservableCollection<ArticleModel>(),
                                    Source = sourceModel
                                };

                                sourceModel.FeedList.Add(feedModel);
                            }
                        }
                        if (sourceModel.FeedList.Any())
                            _sources.Add(sourceModel);
                    }
                }

                return _sources;
            }
        }

        public async Task<SourceModel> GetFavorites()
        {
            var sourceModel = new SourceModel
            {
                SourceConfiguration = new SourceConfigurationModel()
                {
                    SourceNameLong = "Favoriten",
                    SourceNameShort = "Favoriten",
                    Source = SourceEnum.Favorites
                }
            };
            using (var unitOfWork = new UnitOfWork(true))
            {
                var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                var articles = new ObservableCollection<ArticleModel>(await AddModels(await repo.GetByCondition(d => d.IsFavorite, d => d.ChangeDate, true)));
                sourceModel.FeedList = new ObservableCollection<FeedModel>()
                {
                    new FeedModel()
                    {
                        CustomTitle = "Favoriten",
                        ArticleList = articles,
                        FeedConfiguration = new FeedConfigurationModel()
                        {
                            Name = "Favoriten",
                            SourceConfiguration = sourceModel.SourceConfiguration
                        },
                        Source = sourceModel
                    }
                };
            }
            return sourceModel;
        }

        public async Task<ArticleModel> GetArticleById(int articleId)
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                return await AddModels(await repo.GetById(articleId), await unitOfWork.GetDataService());
            }
        }

        public async Task<ObservableCollection<ArticleModel>> GetArticlesByFeed(Guid feedId, int max = 0, int skip = 0)
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                var guidstring = feedId.ToString();
                return new ObservableCollection<ArticleModel>(await AddModels(await repo.GetByCondition(d => d.FeedConfigurationId == guidstring, o => o.PublicationTime, true, max, skip)));
            }
        }


        private List<Task> _aktualizeArticleTasks = new List<Task>();
        private int _newArticles = 0;
        private int _articlesCompleted = 0;
        public async Task ActualizeArticles()
        {
            //Get Total Number of feeds
            int feedcounter = _sources.SelectMany(source => source.FeedList).Count();

            int activecounter = 0;

            foreach (var sourceModel in _sources)
            {
                if (sourceModel.SourceConfiguration.Source == SourceEnum.Favorites)
                    continue;

                foreach (var feed in sourceModel.FeedList)
                {
                    var newfeed = await FeedHelper.Instance.DownloadFeed(feed);
                    _progressService.ShowProgress("Feeds werden heruntergeladen...", Convert.ToInt32((++activecounter * 100) / feedcounter));
                    if (newfeed != null)
                    {
                        ArticleHelper.Instance.AddWordDumpFromFeed(ref newfeed);
                        if (await FeedToDatabase(newfeed))
                            Messenger.Default.Send(feed.FeedConfiguration.Guid, Messages.FeedRefresh);
                    }
                }
            }

            _articlesCompleted = 0;
            using (var unitOfWork = new UnitOfWork(true))
            {
                var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                _newArticles = await repo.CountByCondition(a => a.State == (int)ArticleState.New);
            }

            //Actualize articles
            if (!_aktualizeArticleTasks.Any())
            {
                for (int i = 0; i < 1; i++)
                {
                    var tsk = AktualizeArticlesTask();
                    _aktualizeArticleTasks.Add(tsk);
                }

                foreach (var tsk in _aktualizeArticleTasks)
                {
                    await tsk;
                }
            }
        }

        private async Task AktualizeArticlesTask()
        {
            while (true)
            {
                using (var unitOfWork = new UnitOfWork(false))
                {
                    var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                    var imagerepo = new GenericRepository<ImageModel, ImageEntity>(await unitOfWork.GetDataService());
                    var res = (await repo.GetByCondition(a => a.State == (int)ArticleState.New, d => d.PublicationTime, true, 1)).ToList();
                    if (res.Any())
                    {
                        var article = res[0];
                        article.State = ArticleState.Loading;
                        await repo.Update(article);

                        article.FeedConfiguration = await _settingsRepository.GetFeedConfigurationFor(article.FeedConfigurationId, await unitOfWork.GetDataService());

                        IMediaSourceHelper sh = ArticleHelper.Instance.GetMediaSource(article);
                        if (sh == null)
                        {
                            LogHelper.Instance.Log(LogLevel.Warning, this,
                                "ArticleHelper.DownloadArticle: Tried to Download Article which cannot be downloaded");
                            article.State = ArticleState.WrongSourceFaillure;
                        }
                        else
                            article = await ActualizeArticle(article, sh);


                        await InsertOrUpdateArticleAndTraces(article, await unitOfWork.GetDataService());
                        await unitOfWork.Commit();


                        Messenger.Default.Send(article.Id, Messages.ArticleRefresh);
                        Messenger.Default.Send(article.Id, Messages.FeedArticleRefresh);

                        _progressService.ShowProgress("Artikel werden heruntergeladen...", Convert.ToInt32((++_articlesCompleted * 100) / _newArticles));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public async Task<ArticleModel> GetCompleteArticle(int articleId)
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                var article = await repo.GetById(articleId);
                article = await AddModels(article, await unitOfWork.GetDataService(), true);
                return article;
            }
        }

        public async Task<ObservableCollection<ArticleModel>> GetSimilarCathegoriesArticles(ArticleModel article, int max)
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                List<ThemeArticleRelationModel> list = new List<ThemeArticleRelationModel>();
                foreach (var themeModel in article.Themes)
                {
                    var themeRepo = new GenericRepository<ThemeArticleRelationModel, ThemeArticleRelations>(await unitOfWork.GetDataService());
                    list.AddRange(await themeRepo.GetByCondition(t => t.ThemeId == themeModel.Id));
                }

                var countDic = new Dictionary<int, int>();
                foreach (var themeArticleRelationModel in list)
                {
                    if (themeArticleRelationModel.ArticleId != article.Id)
                    {
                        if (countDic.ContainsKey(themeArticleRelationModel.ArticleId))
                            countDic[themeArticleRelationModel.ArticleId]++;
                        else
                            countDic.Add(themeArticleRelationModel.ArticleId, 1);
                    }
                }
                ObservableCollection<ArticleModel> articles = new ObservableCollection<ArticleModel>();
                var articleRepo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                var favorites = countDic.OrderByDescending(d => d.Value).Select(d => d.Key).ToList();
                for (int i = 0; i < favorites.Count() && i < max; i++)
                {
                    var newart = await articleRepo.GetById(favorites[i]);
                    if (newart != null)
                        articles.Add(newart);
                }

                await AddModels(articles);
                return articles;
            }
        }

        public async Task<ObservableCollection<ArticleModel>> GetSimilarTitlesArticles(ArticleModel article, int max)
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                var keywords = article.WordDump.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                List<int> list = new List<int>();
                foreach (var keyword in keywords)
                {
                    list.AddRange(await (await unitOfWork.GetDataService()).GetByKeyword(keyword));
                }

                var countDic = new Dictionary<int, int>();
                foreach (var id in list)
                {
                    if (id != article.Id)
                    {
                        if (countDic.ContainsKey(id))
                            countDic[id]++;
                        else
                            countDic.Add(id, 1);
                    }
                }

                ObservableCollection<ArticleModel> articles = new ObservableCollection<ArticleModel>();
                var articleRepo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                var favorites = countDic.OrderByDescending(d => d.Value).Select(d => d.Key).ToList();
                for (int i = 0; i < favorites.Count() && i < max; i++)
                {
                    var newart = await articleRepo.GetById(favorites[i]);
                    if (newart != null)
                        articles.Add(newart);
                }

                await AddModels(articles);
                return articles;
            }
        }

        public async Task<ArticleModel> ActualizeArticle(ArticleModel article, IMediaSourceHelper sh)
        {
            try
            {
                if (sh.NeedsToEvaluateArticle())
                {
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

        private async Task<List<ArticleModel>> AddModels(IEnumerable<ArticleModel> models, bool deep = false)
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                var res = new List<ArticleModel>();
                foreach (var articleModel in models)
                {
                    res.Add(await AddModels(articleModel, await unitOfWork.GetDataService(), deep));
                }
                return res;
            }
        }

        private async Task<ArticleModel> AddModels(ArticleModel model, IDataService dataService, bool deep = false)
        {
            try
            {
                var imageRepo = new GenericRepository<ImageModel, ImageEntity>(dataService);
                model.LeadImage = await imageRepo.GetById(model.LeadImageId);

                if (deep)
                {
                    var contentRepo = new GenericRepository<ContentModel, ContentEntity>(dataService);
                    model.Content = await contentRepo.GetByCondition(d => d.ArticleId == model.Id, d => d.Order);
                    foreach (var contentModel in model.Content)
                    {
                        var galleryRepo = new GenericRepository<GalleryModel, GalleryEntity>(dataService);
                        if (contentModel.ContentType == ContentType.Gallery)
                        {
                            contentModel.Gallery = (await galleryRepo.GetByCondition(d => d.Id == contentModel.GalleryId)).FirstOrDefault();
                            if (contentModel.Gallery != null)
                            {
                                contentModel.Gallery.Images = new ObservableCollection<ImageModel>(await imageRepo.GetByCondition(i => i.GalleryId == contentModel.Gallery.Id));
                            }
                        }
                    }

                    model.Themes = await _themeRepository.GetThemesByArticleId(model.Id, dataService);
                    model.RelatedArticles = await GetRelatedArticlesByArticleId(model.Id, dataService);

                    model.FeedConfiguration = await SimpleIoc.Default.GetInstance<ISettingsRepository>().GetFeedConfigurationFor(model.FeedConfigurationId, dataService);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "AddModels failed!", ex);
            }
            return model;
        }

        /// <summary>
        /// returns true if successfull and any article changed
        /// </summary>
        /// <param name="articles"></param>
        /// <returns></returns>
        private async Task<bool> FeedToDatabase(List<ArticleModel> articles)
        {
            try
            {
                if (articles.Any())
                {
                    using (var unitOfWork = new UnitOfWork(false))
                    {
                        var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
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
                        await DeleteAllArticlesAndTrances(oldfeed.Where(d => !d.IsFavorite).Select(d => d.Id).ToList(), await unitOfWork.GetDataService());

                        //only new ones left
                        await InsertAllArticleAndTraces(articles, await unitOfWork.GetDataService());

                        await unitOfWork.Commit();
                        if (articles.Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Exception occured", ex);
            }
            return false;
        }

        private async Task DeleteArticleAndTraces(ArticleModel article)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(false))
                {
                    article.PrepareForSave();

                    var contentRepo = new GenericRepository<ContentModel, ContentEntity>(await unitOfWork.GetDataService());
                    var articleRepo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                    var galleryRepo = new GenericRepository<GalleryModel, GalleryEntity>(await unitOfWork.GetDataService());
                    var imageRepo = new GenericRepository<ImageModel, ImageEntity>(await unitOfWork.GetDataService());

                    var contents = await contentRepo.GetByCondition(d => d.ArticleId == article.Id);
                    foreach (var content in contents)
                    {
                        if (content.ContentType == ContentType.Gallery)
                        {
                            var gallery = (await galleryRepo.GetByCondition(d => d.Id == content.GalleryId)).FirstOrDefault();

                            var images = await imageRepo.GetByCondition(d => d.GalleryId == gallery.Id);
                            foreach (var imageModel in images)
                            {
                                await imageRepo.Delete(imageModel);
                            }
                            await galleryRepo.Delete(gallery);
                        }
                        else if (content.ContentType == ContentType.Image)
                        {
                            var image = (await imageRepo.GetByCondition(d => d.Id == content.ImageId)).FirstOrDefault();
                            await imageRepo.Delete(image);
                        }
                        await contentRepo.Delete(content);
                    }

                    await _themeRepository.SetThemesByArticle(article.Id, new List<int>(), await unitOfWork.GetDataService());
                    await SetRelatedArticlesByArticleId(article.Id, new List<int>(), await unitOfWork.GetDataService());

                    await articleRepo.Delete(article);

                    await unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Article cannot be deleted", ex);
            }
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
                        await _themeRepository.SetThemesByArticle(article.Id, article.Themes.Select(t => t.Id).ToList(), dataService);

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

                    await unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Article cannot be saved", ex);
            }
        }

        private async Task InsertAllArticleAndTraces(List<ArticleModel> newarticles, IDataService dataService)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(false))
                {
                    //insert reihenfolge
                    var images = new List<ImageModel>();
                    //articles

                    var galleries = new List<GalleryModel>();
                    var galleryImages = new List<ImageModel>();
                    var contents = new List<ContentModel>();
                    var removerelationships = new List<ThemeArticleRelationModel>();
                    var addrelationships = new List<ThemeArticleRelationModel>();

                    var articleRepo = new GenericRepository<ArticleModel, ArticleEntity>(dataService);
                    var imageRepo = new GenericRepository<ImageModel, ImageEntity>(dataService);
                    var galleryRepo = new GenericRepository<GalleryModel, GalleryEntity>(dataService);
                    var contentRepo = new GenericRepository<ContentModel, ContentEntity>(dataService);
                    var relationshipsRepo = new GenericRepository<ThemeArticleRelationModel, ThemeArticleRelations>(dataService);

                    foreach (var article in newarticles)
                    {
                        article.PrepareForSave();

                        if (article.LeadImage != null)
                            images.Add(article.LeadImage);

                        if (article.Content != null && article.Content.Any())
                        {
                            foreach (var contentModel in article.Content)
                            {
                                contents.Add(contentModel);

                                if (contentModel.ContentType == ContentType.Gallery && contentModel.Gallery != null)
                                {
                                    galleries.Add(contentModel.Gallery);

                                    if (contentModel.Gallery.Images != null)
                                    {
                                        galleryImages.AddRange(contentModel.Gallery.Images);
                                    }
                                }
                                else if (contentModel.ContentType == ContentType.Image && contentModel.Image != null)
                                {
                                    images.Add(contentModel.Image);
                                }
                            }
                        }
                    }

                    await imageRepo.AddAll(images);
                    await articleRepo.AddAll(newarticles);
                    await galleryRepo.AddAll(galleries);
                    await imageRepo.AddAll(galleryImages);
                    await contentRepo.AddAll(contents);

                    foreach (var articleModel in newarticles)
                    {
                        if (articleModel.Themes != null)
                        {
                            var relationships = await _themeRepository.SetChangesByArticle(articleModel.Id, articleModel.Themes.Select(t => t.Id).ToList(), dataService);

                            removerelationships.AddRange(relationships.Item1);
                            addrelationships.AddRange(relationships.Item2);
                        }
                    }

                    await relationshipsRepo.DeleteAll(addrelationships);
                    await relationshipsRepo.AddAll(addrelationships);

                    /*
                    if (article.RelatedArticles != null)
                        await SetRelatedArticlesByArticleId(article.Id, article.RelatedArticles.Select(t => t.Id).ToList(), dataService);
                    */

                    await unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Article cannot be saved", ex);
            }
        }

        private async Task<bool> SetRelatedArticlesByArticleId(int articleId, List<int> relatedArticles, IDataService dataService)
        {
            var relations = await GetRelatedArticleRelationsByArticleId(articleId, dataService);

            var repo = new GenericRepository<RelatedArticleRelationModel, RelatedArticleRelations>(dataService);
            foreach (var relatedArticleRelationModel in relations)
            {
                if (relatedArticles.Contains(relatedArticleRelationModel.Article1Id))
                    relatedArticles.Remove(relatedArticleRelationModel.Article1Id);
                else if (relatedArticles.Contains(relatedArticleRelationModel.Article2Id))
                    relatedArticles.Remove(relatedArticleRelationModel.Article2Id);
                else
                {
                    if (!await repo.Delete(relatedArticleRelationModel))
                        return false;
                }
            }

            foreach (var article in relatedArticles)
            {
                var model = new RelatedArticleRelationModel()
                {
                    Article1Id = articleId,
                    Article2Id = article
                };

                if (!(await repo.Add(model)))
                    return false;
            }
            return true;
        }

        private async Task<List<ArticleModel>> GetRelatedArticlesByArticleId(int articleId, IDataService dataService)
        {
            var relations = await GetRelatedArticleRelationsByArticleId(articleId, dataService);

            var repo = new GenericRepository<ArticleModel, ArticleEntity>(dataService);
            var res = new List<ArticleModel>();
            foreach (var relatedArticleRelationModel in relations)
            {
                if (relatedArticleRelationModel.Article1Id != articleId)
                {
                    var model = await repo.GetById(relatedArticleRelationModel.Article1Id);
                    if (model != null)
                        res.Add(model);
                }
                else
                {
                    var model = await repo.GetById(relatedArticleRelationModel.Article2Id);
                    if (model != null)
                        res.Add(model);
                }
            }

            return res;
        }

        private async Task<List<RelatedArticleRelationModel>> GetRelatedArticleRelationsByArticleId(int articleId, IDataService dataService)
        {
            var repo = new GenericRepository<RelatedArticleRelationModel, RelatedArticleRelations>(dataService);
            var rel1 = await repo.GetByCondition(d => d.Article1Id == articleId);
            var rel2 = await repo.GetByCondition(d => d.Article2Id == articleId);
            rel1.AddRange(rel2);
            return rel1;
        }


        #region Article Samples

        private ArticleModel GetSampleArticle()
        {
            var avm = new ArticleModel()
            {
                Title = "Auf der Suche nach der nächsten Systemkrise",
                SubTitle = "Liquidität an den Finanzmärkten",
                //Teaser = "Seit dem Ausbruch der Finanzkrise forsten Aufsichtsorgane den Finanzsektor nach möglichen Systemrisiken durch. Grosses Kopfzerbrechen bereitet eine geringere Liquidität.",
                Content = new List<ContentModel>
                {
                    new ContentModel()
                    {
                        Html = "<p>Innert kurzer Zeit schwankten im letzten Oktober die Renditen zehnjähriger US-Staatsanleihen um 37 Basispunkte – was zunächst nicht spektakulär klingt. Man muss sich aber vor Augen führen, dass die prozentualen Preisveränderungen nach dem Kollaps der Investmentbank Lehman Brothers im Jahr 2008 geringer ausgefallen waren. Im Nachgang dieses «flash crash» wurden viele Theorien herumgereicht, warum es zu den schroffen Marktbewegungen gekommen war, obwohl die Nachrichtenlage nicht als besonders schwerwiegend galt. Für viele Kommentatoren und Regulierungsbehörden sind die Vorgänge im Oktober ein Beleg schwindender Liquidität am Anleihemarkt. Vor kurzem reihte sich Jamie Dimon, der Chef der amerikanischen Grossbank JP Morgan, in den Reigen der Warner ein.</p>" +
                        "<h2>Weniger Marktmacher</h2>" +
                        "<p>Märkte sind liquide, wenn Investoren ohne grosse Verzögerungen, zu niedrigen Kosten und zu einem Preis nahe dem Marktwert Wertpapiere kaufen und verkaufen können. Trifft dies nicht zu, steckt Sand im Getriebe. Der Gouverneur der Bank of England, Mark Carney, hatte 2014 darauf hingewiesen, dass beim Obligationenhandel die Zeitdauer, um eine Handelsposition abzuwickeln, heute sieben Mal länger sei als 2008. Unter solchen Bedingungen stellt sich die Frage nach der Effizienz der Märkte.</p>",
                        ContentType = ContentType.Html
                    }
                },
                Author = "Author Maximus",
                LeadImage = new ImageModel() { Url = new Uri("http://images.nzz.ch/eos/v2/image/view/620/-/text/inset/353e98e8/1.18533711/1430496678/tunshikel-park-kathmandu-nepal.jpg") },
                PublicationTime = DateTime.Now,
                State = ArticleState.Loaded
            };
            return avm;
        }
        #endregion
    }
}
