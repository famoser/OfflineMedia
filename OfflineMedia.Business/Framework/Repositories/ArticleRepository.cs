using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.NMModels;
using OfflineMedia.Business.Sources;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Services.Interfaces;
using OfflineMedia.Data;
using OfflineMedia.Data.Entities;

namespace OfflineMedia.Business.Framework.Repositories
{
    public partial class ArticleRepository : IArticleRepository
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
            return GetInfoArticle();
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

        public void UpdateArticleFlat(ArticleModel am)
        {
            _toDatabaseFlatArticles.Add(am);
#pragma warning disable 4014
            ToDatabase();
#pragma warning restore 4014
        }

        private async Task UpdateArticleFlat(ArticleModel am, IDataService dataService)
        {
            var repo = new GenericRepository<ArticleModel, ArticleEntity>(dataService);
            await repo.Update(am);
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

                    model.Themes = await _themeRepository.GetThemesByArticleId(model.Id);
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
                            var relationships = await _themeRepository.SetChangesByArticle(articleModel.Id, articleModel.Themes.Select(t => t.Id).ToList());

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
            var avm = new ArticleModel
            {
                Title = "Auf der Suche nach der nächsten Systemkrise",
                SubTitle = "Liquidität an den Finanzmärkten",
                //Teaser = "Seit dem Ausbruch der Finanzkrise forsten Aufsichtsorgane den Finanzsektor nach möglichen Systemrisiken durch. Grosses Kopfzerbrechen bereitet eine geringere Liquidität.",
                Content = new List<ContentModel>
                {
                    new ContentModel
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
