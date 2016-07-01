using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Helpers.Text;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.ContentModels;
using OfflineMedia.Business.Newspapers.Stern.Models;
using OfflineMedia.Business.Repositories.Interfaces;

namespace OfflineMedia.Business.Newspapers.Stern
{
    public class SternHelper : BaseMediaSourceHelper
    {

        public ArticleModel FeedToArticleModel(Entry2 nfa, SourceConfigurationModel scm)
        {
            if (nfa == null) return null;

            try
            {
                if (nfa.articleType == "standard-article")
                {
                    var a = new ArticleModel
                    {
                        PublicationTime = DateTime.Parse(nfa.timestamp),
                        Title = nfa.kicker,
                        SubTitle = nfa.headline,
                        Teaser = nfa.teaser,
                    };

                    if (nfa.images != null && nfa.images.Count > 3)
                        a.LeadImage = new ImageModel()
                        {
                            Url = new Uri(nfa.images[3].src)
                        };

                    a.LogicUri = new Uri(scm.LogicBaseUrl + nfa.contentId + ".json");
                    a.PublicUri = new Uri(scm.PublicBaseUrl + nfa.contentId);

                    return a;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "SternHelper.FeedToArticleModel failed", this, ex);
            }
            return null;
        }

        public override async Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            if (article == null) return new Tuple<bool, ArticleModel>(false, am);

            try
            {
                article = article.Replace("[[]]", "[]");
                var a = JsonConvert.DeserializeObject<SternArticle>(article);
                return await ArticleToArticleModel(a, am);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "NzzHelper.EvaluateArticle failed", this, ex);
            }
            return new Tuple<bool, ArticleModel>(false, am);
        }

        public override bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            original.Content = evaluatedArticle.Content;
            original.Author = evaluatedArticle.Author;
            original.Themes = evaluatedArticle.Themes;
            return true;
        }

        private async Task<Tuple<bool, ArticleModel>> ArticleToArticleModel(SternArticle na, ArticleModel am)
        {
            if (na != null)
            {
                try
                {
                    am.Content.Add(new TextContentModel()
                    {
                        Content = HtmlConverter.HtmlToParagraph(GetHtml(na.content))
                    });

                    if (na.head != null && na.head.credits != null)
                    {
                        am.Author = na.head.credits.author;
                    }
                    
                    await AddThemesAsync(am, null);

                    return new Tuple<bool, ArticleModel>(true, am);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, "SternHelper.ArticleToArticleModel deserialization failed", this, ex);
                }
            }
            return new Tuple<bool, ArticleModel>(false, am);
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
                                entry.entries.Select(rawarticles => FeedToArticleModel(rawarticles, scm))
                                    .Where(article => article != null));
                        }
                    }
                }
                return articlelist;

            });
        }

        public override Task<bool> EvaluateArticle(ArticleModel articleModel)
        {
            throw new NotImplementedException();
        }
    }
}
