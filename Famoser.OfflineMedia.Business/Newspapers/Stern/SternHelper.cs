using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Newspapers.Stern.Models;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Business.Newspapers.Stern
{
    public class SternHelper : BaseMediaSourceHelper
    {

        public ArticleModel FeedToArticleModel(Entry2 nfa, FeedModel scm)
        {
            if (nfa == null) return null;

            return ExecuteSafe(() =>
            {
                if (nfa.articleType == "standard-article")
                {
                    var a = ConstructArticleModel(scm);
                    a.PublishDateTime = DateTime.Parse(nfa.timestamp);
                    a.Title = nfa.kicker;
                    a.SubTitle = nfa.headline;
                    a.Teaser = nfa.teaser;


                    if (nfa.images != null && nfa.images.Count > 3)
                        a.LeadImage = new ImageContentModel()
                        {
                            Url = nfa.images[3].src
                        };

                    a.LogicUri = scm.Source.LogicBaseUrl + nfa.contentId + ".json";
                    a.PublicUri = scm.Source.PublicBaseUrl + nfa.contentId;

                    return a;
                }
                return null;
            });
        }

        private async Task<bool> ArticleToArticleModel(SternArticle na, ArticleModel am)
        {
            am.Content.Add(new TextContentModel()
            {
                Content = HtmlConverter.CreateOnce().HtmlToParagraph(GetHtml(na.content))
            });

            if (na.head != null && na.head.credits != null)
                am.Author = na.head.credits.author;

            if (string.IsNullOrWhiteSpace(am.Author))
                am.Author = am.Feed.Source.Name;


            await AddThemesAsync(am);

            return true;
        }

        private string GetHtml(List<Content> children)
        {
            var res = "";
            if (children != null)
                foreach (var child in children)
                {
                    if (child.type == "p")
                        res += "<p>" + GetHtml(child.children) + "</p>";
                    else if (child.type == "strong")
                        res += "<strong>" + GetHtml(child.children) + "</strong>";
                    else if (child.type == "text")
                        res += child.content;
                    else if (child.type == "a")
                        res += "<a href=\"" + child.href + "\">" + GetHtml(child.children) + "</a>";
                }


            return res;
        }

        private string GetHtml(List<Content2> children)
        {
            var res = "";
            if (children != null)
                foreach (var child in children)
                {
                    if (child.type == "p")
                        res += "<p>" + GetHtml(child.children) + "</p>";
                    else if (child.type == "strong")
                        res += "<strong>" + GetHtml(child.children) + "</strong>";
                    else if (child.type == "text")
                        res += child.content;
                    else if (child.type == "a")
                        res += "<a href=\"" + child.href + "\">" + GetHtml(child.children) + "</a>";
                }


            return res;
        }

        public SternHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {

                var articlelist = new List<ArticleModel>();
                var feed = await DownloadAsync(feedModel);
                if (feed != null)
                {
                    var f = JsonConvert.DeserializeObject<SternFeed>(feed);
                    foreach (var entry in f.entries)
                    {
                        if (entry.type == "teaserlist")
                        {
                            articlelist.AddRange(
                                entry.entries.Select(rawarticles => FeedToArticleModel(rawarticles, feedModel))
                                    .Where(article => article != null));
                        }
                    }
                }
                return articlelist;

            });
        }

        public override Task<bool> EvaluateArticle(ArticleModel articleModel)
        {
            return ExecuteSafe(async () =>
            {
                var article = await DownloadAsync(articleModel);

                article = article.Replace("[[]]", "[]");
                var a = JsonConvert.DeserializeObject<SternArticle>(article);
                return await ArticleToArticleModel(a, articleModel);
            });
        }
    }
}
