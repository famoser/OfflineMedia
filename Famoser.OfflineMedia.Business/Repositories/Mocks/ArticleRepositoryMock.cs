using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Data.Enums;

#pragma warning disable 1998
namespace Famoser.OfflineMedia.Business.Repositories.Mocks
{
    public class ArticleRepositoryMock : IArticleRepository
    {
        public ArticleRepositoryMock()
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
                IsActive = true
            };
            var sm2 = new SourceModel()
            {
                Abbreviation = "BAZ",
                Guid = Guid.NewGuid(),
                Source = Sources.Nzz,
                Name = "Basler Zeitung",
                LogicBaseUrl = "http://api.nzz.ch/",
                PublicBaseUrl = "http://nzz.ch/",
                IsActive = false
            };
            var fm = new FeedModel()
            {
                Name = "Home",
                Guid = Guid.NewGuid(),
                Source = sm,
                Url = "home",
                AllArticles =
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
                AllArticles =
                {
                    article,
                    article2,
                    article
                }
            };
            sm.ActiveFeeds.Add(fm);
            sm.AllFeeds.Add(fm);
            sm.AllFeeds.Add(fm2);
            sm.AllFeeds.Add(fm2);

            _sources.Add(sm);
            _sources.Add(sm);
            _sources.Add(sm2);
            _sources.Add(sm2);
            _sources.Add(sm);
        }

        private readonly ObservableCollection<SourceModel> _sources = new ObservableCollection<SourceModel>(); 
        public ObservableCollection<SourceModel> GetActiveSources()
        {
            return _sources;
        }

        public ObservableCollection<SourceModel> GetAllSources()
        {
            return _sources;
        }

        public async Task<bool> LoadFullArticleAsync(ArticleModel am)
        {
            am.Content.Add(new TextContentModel()
            {
                Content = HtmlConverter.CreateOnce("").HtmlToParagraph("<h1>This is a sample Article Content</h1>" +
                                                        "<p>A paragraph which explains more or less interesting stuff.</p>")
            });
            return true;
        }

        public async Task<bool> LoadFullFeedAsync(FeedModel fm)
        {
            var article = GetSampleArticle();
            article.Feed = fm;
            fm.AllArticles.Add(article);
            fm.AllArticles.Add(article);
            fm.AllArticles.Add(article);
            fm.AllArticles.Add(article);
            fm.AllArticles.Add(article);
            fm.AllArticles.Add(article);
            fm.AllArticles.Add(article);
            return true;
        }

        public async Task ActualizeAllArticlesAsync()
        {

        }

        public async Task ActualizeArticleAsync(ArticleModel am)
        {

        }

        public async Task<bool> SetArticleFavoriteStateAsync(ArticleModel am, bool isFavorite)
        {
            am.IsFavorite = isFavorite;
            return true;
        }

        public async Task<bool> MarkArticleAsReadAsync(ArticleModel am)
        {
            am.IsRead = true;
            return true;
        }

        public async Task<bool> SetFeedActiveStateAsync(FeedModel feedModel, bool isActive)
        {
            return true;
        }

        public async Task<bool> SetSourceActiveStateAsync(SourceModel sourceModel, bool isActive)
        {
            return true; 
        }

        public async Task<bool> SwitchFeedActiveStateAsync(FeedModel feedModel)
        {
            return true;
        }

        public async Task<bool> SwitchSourceActiveStateAsync(SourceModel sourceModel)
        {
            return true;
        }

        public ArticleModel GetInfoArticle()
        {
            return GetSampleArticle();
        }

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
                Content = HtmlConverter.CreateOnce("").HtmlToParagraph("<h1>Über diese App</h1>" +
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
    }
}
