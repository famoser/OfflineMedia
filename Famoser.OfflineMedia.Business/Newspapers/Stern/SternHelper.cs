﻿using System;
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
                        Content = HtmlConverter.CreateOnce().HtmlToParagraph("<p>Content cannot be displayed (invalid json content)</p>")
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

            var currentIndex = 0;
            while (json.Substring(currentIndex).Contains("\"content\":"))
            {
                currentIndex += json.Substring(currentIndex).IndexOf("\"content\":", StringComparison.Ordinal) + "\"content\":".Length;

                //skipfirst
                currentIndex += json.Substring(currentIndex).IndexOf("\"", StringComparison.Ordinal) + 1;

                while (true)
                {
                    currentIndex += json.Substring(currentIndex).IndexOf("\"", StringComparison.Ordinal);
                    //check if escaped
                    if (json[currentIndex - 1] != Convert.ToChar("\\"))
                    {
                        //confirm it is not last
                        if (json.Substring(currentIndex + 1).IndexOf("\"", StringComparison.Ordinal) <
                            json.Substring(currentIndex + 1).IndexOf("}", StringComparison.Ordinal))
                        {
                            //insert escape caracter
                            json = json.Insert(currentIndex, "\\");
                        }
                        else
                        {
                            currentIndex += json.Substring(currentIndex).IndexOf("}", StringComparison.Ordinal);
                            break;
                        }
                    }
                }

            }

            return json;
            //var currentIndex = 0;
            //while (true)
            //{
            //    var start = json.IndexOf("\"content\": \"", currentIndex);
            //    if (start > 0)
            //    {
            //        start += "\"content\": \"".Length;
            //        var end = json.LastIndexOf("\"", start);
            //        var content = json.Substring(start, end - start);
            //        content = Regex.Replace(content, "[^\\]\"", "\\\""); //replace " with \" if it is not already escaped
            //        json = json.Substring(0, start) + content + json.Substring(end);
            //        currentIndex = end;
            //    }
            //    else
            //    {
            //        return json;
            //    }
            //}
        }
    }
}
