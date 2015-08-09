using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Enums.Models;
using OfflineMediaV3.Business.Framework.Generic;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Helpers;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Models.NewsModel.NMModels;
using OfflineMediaV3.Business.Sources.Blick;
using OfflineMediaV3.Business.Sources.Nzz;
using OfflineMediaV3.Business.Sources.Postillon;
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

        public ArticleRepository(ISettingsRepository settingsRepository, IThemeRepository themeRepository)
        {
            _settingsRepository = settingsRepository;
            _themeRepository = themeRepository;
        }

        public ObservableCollection<SourceModel> GetSampleArticles()
        {
            var oc = new ObservableCollection<SourceModel>();
            for (int i = 0; i < 3; i++)
            {
                var sourceModel = new SourceModel()
                {
                    Configuration = new SourceConfigurationModel()
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
                            GetSampleArticle(),
                            GetSampleArticle(),
                            GetSampleArticle(),
                            GetSampleArticle(),
                        },
                        ShortArticleList = new ObservableCollection<ArticleModel>()
                        {

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
                        Type = ContentType.Html,
                        Html = "<h1>Über diese App</h1>" +
                                "<p>OfflineMedia erlaubt das Lesen der öffentlichen Onlineausgabe von Zeitungen, auch wenn gerade kein Internet verfügbar ist. " +
                                "<br /><br />" +
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
                                "<p>Mein Name ist Florian Moser, ich bin ein junger Programmierer aus Allschwil, Schweiz. " +
                                "<br /><br />" +
                                "Diese App soll die wichtigsten Medien der Schweiz auf Windows Phone bringen, in einer einzigen App kombiniert." +
                                "<br /><br />" +
                                "Neben Apps entwickle ich auch Webseiten. Ein Kontaktformular und Informationen über meine Projekte sind auf meiner Webseite zu finden.</p>" +
                                "<p><b>Webseite:</b> florianalexandermoser.ch<br />" +
                                "<b>E-Mail:</b> OfflineMedia@outlook.com</p>"
                    }
                },
                Title = "Info",
                SubTitle = "Informationen über diese App"
            };
        }

        public async Task UpdateArticle(ArticleModel article)
        {
            using (var unitOfWork = new UnitOfWork(false))
            {
                await InsertOrUpdateArticleAndTraces(article, await unitOfWork.GetDataService());
            }
        }

        private readonly ObservableCollection<SourceModel> _sources = new ObservableCollection<SourceModel>();
        public async Task<ObservableCollection<SourceModel>> GetSources()
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                var sources = await _settingsRepository.GetSourceConfigurations(await unitOfWork.GetDataService());

                foreach (var source in sources)
                {
                    if (source.BoolValue)
                    {
                        var sourceModel = new SourceModel()
                        {
                            Configuration = source,
                            FeedList = new ObservableCollection<FeedModel>(),
                        };
                        foreach (var feedConfigurationModel in source.Feeds)
                        {
                            if (feedConfigurationModel.BoolValue)
                            {
                                var feedModel = new FeedModel()
                                {
                                    FeedConfiguration = feedConfigurationModel,
                                    ArticleList = new ObservableCollection<ArticleModel>(),
                                    ShortArticleList = new ObservableCollection<ArticleModel>()
                                };

                                var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());

                                var guidString = feedConfigurationModel.Guid.ToString();
                                feedModel.ShortArticleList =
                                    new ObservableCollection<ArticleModel>(
                                        await
                                            repo.GetByCondition(d => d.FeedId == guidString, d => d.PublicationTime,
                                                true, 5));
                                feedModel.Source = sourceModel;

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

        public async Task<ObservableCollection<ArticleModel>> GetArticlesByFeed(Guid feedId, int max = 0)
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                var guidstring = feedId.ToString();
                return new ObservableCollection<ArticleModel>(await AddModels(await repo.GetByCondition(d => d.FeedId == guidstring, o => o.PublicationTime, true, max)));
            }
        }

        public async Task ActualizeArticles(IProgressService progressService)
        {
            //Get Total Number of feeds
            int feedcounter = _sources.SelectMany(source => source.FeedList).Count();

            int activecounter = 0;

            foreach (var sourceModel in _sources)
            {
                if (sourceModel.Configuration.Source == SourceEnum.Favorites)
                    continue;

                foreach (var feed in sourceModel.FeedList)
                {
                    var newfeed = await FeedHelper.Instance.DownloadFeed(feed);
                    progressService.ShowProgress("Feeds werden heruntergeladen...", Convert.ToInt32((++activecounter * 100) / feedcounter));

                    await FeedToDatabase(feed.FeedConfiguration.Guid, newfeed);
                    Messenger.Default.Send(feed.FeedConfiguration.Guid, Messages.FeedRefresh);
                }
            }

            activecounter = 0;
            int maxCounter;
            using (var unitOfWork = new UnitOfWork(true))
            {
                var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                maxCounter = await repo.CountByCondition(a => a.State == (int)ArticleState.New);
            }

            //Actualize articles
            while (activecounter < maxCounter)
            {
                using (var unitOfWork = new UnitOfWork(false))
                {
                    var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                    var res = (await repo.GetByCondition(a => a.State == (int) ArticleState.New, d => d.PublicationTime, true, 1)).ToList();
                    if (res.Any())
                    {
                        var article = res[0];
                        article.State = ArticleState.Loading;
                        await repo.AddOrUpdate(article);

                        article.SourceConfiguration = await _settingsRepository.GetSourceConfigurationFor(article.SourceId, await unitOfWork.GetDataService());
                        await ActualizeArticle(article);
                        await InsertOrUpdateArticleAndTraces(article, await unitOfWork.GetDataService());
                        await unitOfWork.Commit();

                        Messenger.Default.Send(article.Id, Messages.ArticleRefresh);
                    }
                }
                progressService.ShowProgress("Artikel werden heruntergeladen...", Convert.ToInt32((++activecounter * 100) / maxCounter));
            }
            progressService.HideProgress();
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

        public async Task<ArticleModel> ActualizeArticle(ArticleModel article)
        {
            try
            {
                if (article.SourceConfiguration != null && article.SourceConfiguration.Source == SourceEnum.Nzz)
                {
                    string articleresult = await Download.DownloadStringAsync(article.LogicUri);
                    var nzzh = new NzzHelper();
                    article = nzzh.EvaluateArticle(articleresult, article.SourceConfiguration);
                }
                else if (article.SourceConfiguration != null && (article.SourceConfiguration.Source == SourceEnum.Blick || article.SourceConfiguration.Source == SourceEnum.BlickAmAbend))
                {
                    string articleresult = await Download.DownloadStringAsync(article.LogicUri);
                    var blickh = new BlickHelper();
                    article = blickh.EvaluateArticle(articleresult, article.SourceConfiguration);
                }
                else if (article.SourceConfiguration != null && article.SourceConfiguration.Source == SourceEnum.Postillon)
                {
                    string articleresult = await Download.DownloadStringAsync(article.LogicUri);
                    var postillonh = new PostillonHelper();
                    article = postillonh.EvaluateArticle(articleresult, article.SourceConfiguration);
                }
                else
                {
                    LogHelper.Instance.Log(LogLevel.Warning, this, "ArticleHelper.DownloadArticle: Tried to Download Article which cannot be downloaded");
                    article.State = ArticleState.WrongSourceFaillure;
                    return article;
                }
                article.State = ArticleState.Loaded;
                ArticleHelper.Instance.OptimizeArticle(ref article);
                return article;
            }
            catch (Exception ex)
            {
                if (article != null)
                    LogHelper.Instance.Log(LogLevel.Error, this, "ActualizeArticle failed! Source: " + article.SourceId + " URL: " + article.PublicUri, ex);
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
                        if (contentModel.Type == ContentType.Gallery)
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
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "AddModels failed!", ex);
            }
            return model;
        }

        private async Task FeedToDatabase(Guid feedId, List<ArticleModel> articles)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(false))
                {
                    var repo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                    var guidString = feedId.ToString();
                    var oldfeed = await repo.GetByCondition(a => a.FeedId == guidString);
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
                    foreach (var articleEntity in oldfeed)
                    {
                        if (!articleEntity.IsFavorite)
                            await DeleteArticleAndTraces(articleEntity);
                    }

                    //only new ones left
                    foreach (var articleModel in articles)
                    {
                        await InsertOrUpdateArticleAndTraces(articleModel, await unitOfWork.GetDataService());
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Exception occured", ex);
                throw;
            }
        }

        private async Task DeleteArticleAndTraces(ArticleModel article)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(false))
                {
                    var contentRepo = new GenericRepository<ContentModel, ContentEntity>(await unitOfWork.GetDataService());
                    var articleRepo = new GenericRepository<ArticleModel, ArticleEntity>(await unitOfWork.GetDataService());
                    var galleryRepo = new GenericRepository<GalleryModel, GalleryEntity>(await unitOfWork.GetDataService());
                    var imageRepo = new GenericRepository<ImageModel, ImageEntity>(await unitOfWork.GetDataService());

                    var contents = await contentRepo.GetByCondition(d => d.ArticleId == article.Id);
                    foreach (var content in contents)
                    {
                        if (content.Type == ContentType.Gallery)
                        {
                            var gallery = (await galleryRepo.GetByCondition(d => d.Id == content.GalleryId)).FirstOrDefault();

                            var images = await imageRepo.GetByCondition(d => d.GalleryId == gallery.Id);
                            foreach (var imageModel in images)
                            {
                                await imageRepo.Delete(imageModel);
                            }
                            await galleryRepo.Delete(gallery);
                        }
                        else if (content.Type == ContentType.Image)
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

        private async Task InsertOrUpdateArticleAndTraces(ArticleModel article, IDataService dataService)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(false))
                {
                    var articleRepo = new GenericRepository<ArticleModel, ArticleEntity>(dataService);
                    var imageRepo = new GenericRepository<ImageModel, ImageEntity>(dataService);
                    var galleryRepo = new GenericRepository<GalleryModel, GalleryEntity>(dataService);
                    var contentRepo = new GenericRepository<ContentModel, ContentEntity>(dataService);

                    if (article.LeadImage != null && article.LeadImage.Id == 0)
                        article.LeadImage.Id = await imageRepo.AddOrUpdate(article.LeadImage);

                    article.Id  = await articleRepo.AddOrUpdate(article);

                    if (article.Content != null && article.Content.Any())
                    {
                        foreach (var contentModel in article.Content)
                        {
                            contentModel.ArticleId = article.Id;

                            if (contentModel.Type == ContentType.Gallery && contentModel.Gallery != null)
                            {
                                contentModel.Gallery.Id = await galleryRepo.AddOrUpdate(contentModel.Gallery);

                                if (contentModel.Gallery.Images != null)
                                {
                                    foreach (var imageModel in contentModel.Gallery.Images)
                                    {
                                        imageModel.GalleryId = contentModel.Gallery.Id;
                                        imageModel.Id = await imageRepo.AddOrUpdate(imageModel);
                                    }
                                }
                            }
                            else if (contentModel.Type == ContentType.Image && contentModel.Image != null)
                            {
                                contentModel.Image.Id = await imageRepo.AddOrUpdate(contentModel.Image);
                            }

                            contentModel.Id = await contentRepo.AddOrUpdate(contentModel);
                        }
                    }

                    if (article.Themes != null)
                        await _themeRepository.SetThemesByArticle(article.Id, article.Themes.Select(t => t.Id).ToList(), dataService);

                    if (article.RelatedArticles != null)
                        await SetRelatedArticlesByArticleId(article.Id, article.RelatedArticles.Select(t => t.Id).ToList(), dataService);

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

                if ((await repo.AddOrUpdate(model)) == -1)
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
        private ArticleModel GetDummyArticle()
        {
            var avm = new ArticleModel()
            {
                Title = "Keine Artikel",
                SubTitle = "Es wurden noch keine Artikel heruntergeladen / hinzugefügt",
                Teaser = "Um Artikel herunterzuladen wird eine Internetverbindung benötigt",
                Content = new List<ContentModel>()
                {
                    new ContentModel()
                    {
                        Html = "<p>Um Artikel herunterzuladen wird eine Internetverbindung benötigt</p>", 
                        Type = ContentType.Html
                    }
                }
            };
            return avm;
        }

        private ArticleModel GetSampleArticle()
        {
            var avm = new ArticleModel()
            {
                Title = "Auf der Suche nach der nächsten Systemkrise",
                SubTitle = "Liquidität an den Finanzmärkten",
                Teaser = "Seit dem Ausbruch der Finanzkrise forsten Aufsichtsorgane den Finanzsektor nach möglichen Systemrisiken durch. Grosses Kopfzerbrechen bereitet eine geringere Liquidität.",
                Content = new List<ContentModel>
                {
                    new ContentModel()
                    {
                        Html = "<p>Innert kurzer Zeit schwankten im letzten Oktober die Renditen zehnjähriger US-Staatsanleihen um 37 Basispunkte – was zunächst nicht spektakulär klingt. Man muss sich aber vor Augen führen, dass die prozentualen Preisveränderungen nach dem Kollaps der Investmentbank Lehman Brothers im Jahr 2008 geringer ausgefallen waren. Im Nachgang dieses «flash crash» wurden viele Theorien herumgereicht, warum es zu den schroffen Marktbewegungen gekommen war, obwohl die Nachrichtenlage nicht als besonders schwerwiegend galt. Für viele Kommentatoren und Regulierungsbehörden sind die Vorgänge im Oktober ein Beleg schwindender Liquidität am Anleihemarkt. Vor kurzem reihte sich Jamie Dimon, der Chef der amerikanischen Grossbank JP Morgan, in den Reigen der Warner ein.</p>" +
                        "<h2>Weniger Marktmacher</h2>" + 
                        "<p>Märkte sind liquide, wenn Investoren ohne grosse Verzögerungen, zu niedrigen Kosten und zu einem Preis nahe dem Marktwert Wertpapiere kaufen und verkaufen können. Trifft dies nicht zu, steckt Sand im Getriebe. Der Gouverneur der Bank of England, Mark Carney, hatte 2014 darauf hingewiesen, dass beim Obligationenhandel die Zeitdauer, um eine Handelsposition abzuwickeln, heute sieben Mal länger sei als 2008. Unter solchen Bedingungen stellt sich die Frage nach der Effizienz der Märkte.</p>", 
                        Type = ContentType.Html
                    }
                },
                Author = "Author Maximus",
                LeadImage = new ImageModel() { Url = new Uri("http://images.nzz.ch/eos/v2/image/view/620/-/text/inset/353e98e8/1.18533711/1430496678/tunshikel-park-kathmandu-nepal.jpg") },
                PublicationTime = DateTime.Now
            };
            return avm;
        }
        #endregion
    }
}
