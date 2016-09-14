using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
                    var lowkicker = nfa.kicker.ToLower().Trim();
                    if (lowkicker.EndsWith("quiz") || nfa.kicker.Contains("test"))
                        return null;

                    var blockedIds = new[]
                    {
                        "6979480", //persönlichkeitstest
                        "6974764", //Quiz
                        "6948896", //Europa Quiz
                        "6979480", //persönlichkeitstest
                        "6979480", //persönlichkeitstest
                    };

                    if (blockedIds.Any(i => i == nfa.contentId))
                        return null;

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
            am.Content.Clear();
            var p = HtmlConverter.CreateOnce(am.Feed.Source.PublicBaseUrl).HtmlToParagraph(GetHtml(na.content));
            if (p != null && p.Any())
                am.Content.Add(new TextContentModel()
                {
                    Content = p
                });

            if (!am.Content.Any())
                am.Content.Add(TextHelper.TextToTextModel("Öffnen Sie den Artikel in Ihrem Browser für mehr Informationen."));

            if (na.head != null && na.head.credits != null)
                am.Author = na.head.credits.author;

            if (string.IsNullOrWhiteSpace(am.Author))
                am.Author = am.Feed.Source.Name;

            am.Themes.Clear();
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
                article = RepairInvalidContent(article);
                try
                {
                    var a = JsonConvert.DeserializeObject<SternArticle>(article);
                    return await ArticleToArticleModel(a, articleModel);
                }
                catch (Exception ex)
                {
                    articleModel.Content.Add(new TextContentModel()
                    {
                        Content = HtmlConverter.CreateOnce(articleModel.Feed.Source.PublicBaseUrl).HtmlToParagraph("<p>Content cannot be displayed (invalid json content)</p>")
                    });
                    articleModel.Author = "Stern";

                    await AddThemesAsync(articleModel);

                    return true;
                }
            });
        }

        //the stern API makers were not smart enough for json :/ They do not escape " correctly everytime. wontfix, cause stern is a shit newspaper anyways
        private string RepairInvalidContent(string json)
        {
            json = WebUtility.HtmlDecode(json);
            json = json.Replace("[[]]", "[]");

            //remove twitter shit cause they do not escape it properly
            //,{"type":"twitterElement","content":"*"}
            var replacePatterns = new[]
            {",{\"type\":\"(twitterElement|instagramElement)\",\"content\":\"([<a-zA-Z =\\\\\"->?@—!_])+}"};
            foreach (var replacePattern in replacePatterns)
            {
                json = Regex.Replace(json, replacePattern, "");
            }

            return json;
        }
    }
}
