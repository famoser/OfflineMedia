using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.OfflineMedia.Business.Enums.Models.TextModels;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;
using Famoser.OfflineMedia.Business.Newspapers.Nzz.Models;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Business.Newspapers.Nzz
{
    public class NzzHelper : BaseMediaSourceHelper
    {
        public ArticleModel FeedToArticleModel(NzzFeedArticle nfa, FeedModel scm)
        {
            if (nfa == null || !nfa.path.Contains("/api/")) return null;

            try
            {
                var a = ConstructArticleModel(scm);
                a.PublishDateTime = nfa.publicationDateTime;
                a.Title = nfa.title;
                a.SubTitle = nfa.subTitle;
                if (string.IsNullOrWhiteSpace(a.SubTitle) && nfa.title == "Was heute wichtig ist")
                    a.SubTitle = "Dieser Artikel wird laufend aktualisiert";

                a.LeadImage = LeadImageToImage(nfa.leadImage, scm);

                a.LogicUri = scm.Source.LogicBaseUrl + nfa.path.Substring("/api/".Length);
                var guid = nfa.path.Substring(nfa.path.LastIndexOf("/", StringComparison.Ordinal) + 1);
                a.PublicUri = scm.Source.PublicBaseUrl + guid;

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "NzzHelper.FeedToArticleModel failed", this, ex);
                return null;
            }
        }


        private Task<bool> ArticleToArticleModel(NzzArticle na, ArticleModel am)
        {
            return ExecuteSafe(async () =>
            {

                for (int i = 0; i < na.body.Length; i++)
                {
                    if (na.body[i].style == "h4")
                        na.body[i].style = "h2";
                    if (na.body[i].style == "h3")
                        na.body[i].style = "h1";
                    string starttag = "<" + na.body[i].style + ">";
                    string endtag = "</" + na.body[i].style + ">";
                    if (string.IsNullOrWhiteSpace(na.body[i].text))
                    {
                        foreach (var nzzBox in na.body[i].boxes)
                        {
                            if (nzzBox.type == "image")
                            {
                                am.Content.Add(new ImageContentModel()
                                {
                                    Url = nzzBox.path,
                                    Text = TextHelper.TextToTextModel(nzzBox.caption)
                                });
                            }
                            else if (nzzBox.type == "video" || nzzBox.type == "html")
                            {
                                //dont do shit
                            }
                            else if (nzzBox.type == "infobox")
                            {
                                var newContent = HtmlConverter.CreateOnce(am.Feed.Source.PublicBaseUrl).HtmlToParagraph("<p>" + nzzBox.body + "</p>");

                                foreach (var paragraphModel in newContent)
                                {
                                    var ntm = new TextModel()
                                    {
                                        Children = paragraphModel.Children,
                                        TextType = TextType.Cursive
                                    };
                                    paragraphModel.Children = new List<TextModel> { ntm };
                                }
                                if (!string.IsNullOrWhiteSpace(nzzBox.title))
                                    newContent.Insert(0, new ParagraphModel()
                                    {
                                        ParagraphType = ParagraphType.Title,
                                        Children = new List<TextModel>()
                                    {
                                        new TextModel()
                                        {
                                            Text = nzzBox.title,
                                            TextType = TextType.Cursive
                                        }
                                    }
                                    });
                                if (newContent.Any())
                                    am.Content.Add(new TextContentModel()
                                    {
                                        Content = newContent
                                    });
                                else
                                    "wat".ToString();
                            }
                            else
                                LogHelper.Instance.LogInfo("nzz content type not found: " + nzzBox.mimeType, this);
                        }
                    }
                    else
                    {
                        if (!na.body[i].text.StartsWith("Mehr zum Thema"))
                        {
                            var content = HtmlConverter.CreateOnce(am.Feed.Source.PublicBaseUrl).HtmlToParagraph(starttag + na.body[i].text + endtag);
                            if (content != null && content.Count > 0)
                                am.Content.Add(new TextContentModel()
                                {
                                    Content = content
                                });
                        }
                    }
                }

                if (!am.Content.Any())
                    am.Content.Add(TextHelper.TextToTextModel("Der Inhalt dieses Artikels wird nicht unterstützt. Öffne den Artikel im Browser um mehr zu sehen."));

                if (na.authors != null)
                    foreach (var nzzAuthor in na.authors)
                    {
                        if (!string.IsNullOrEmpty(nzzAuthor.name))
                        {
                            am.Author = nzzAuthor.name;
                            if (!string.IsNullOrEmpty(nzzAuthor.abbreviation))
                                am.Author += ", " + nzzAuthor.abbreviation;
                        }
                        else
                            am.Author = nzzAuthor.abbreviation;
                    }

                if (!string.IsNullOrEmpty(na.agency))
                    am.Author += " " + na.agency;

                if (string.IsNullOrWhiteSpace(am.Author))
                    am.Author = "NZZ";

                if (!string.IsNullOrEmpty(na.leadText))
                    am.Teaser = na.leadText;

                await AddThemesAsync(am, na.departments);

                return true;
            });
        }

        private ImageContentModel LeadImageToImage(NzzLeadImage li, FeedModel feedModel)
        {
            if (li != null)
            {
                try
                {
                    var img = new ImageContentModel()
                    {
                        Text = new TextContentModel()
                        {
                            Content = HtmlConverter.CreateOnce(feedModel.Source.PublicBaseUrl).HtmlToParagraph(li.caption)
                        }
                    };
                    if (li.path.Contains("http://nzz-img.s3.amazonaws.com/"))
                    {
                        var uri = li.path.Substring(li.path.IndexOf("http://nzz-img.s3.amazonaws.com/", StringComparison.Ordinal));
                        if (!uri.Contains("height"))
                            img.Url = uri;
                    }
                    else
                    {
                        var uri = li.path.Replace("%width%", "640").Replace("%height%", "360")
                            .Replace("%format%", "text");
                        img.Url = uri;
                    }
                    return img;
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, "NzzHelper.LeadImageToImage deserialization failed", this, ex);
                    return null;
                }
            }
            return null;
        }

        public NzzHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {
                var articlelist = new List<ArticleModel>();
                var feed = await DownloadAsync(feedModel);
                var f = JsonConvert.DeserializeObject<NzzFeed>(feed);
                articlelist.AddRange(f.articles.Select(am => FeedToArticleModel(am, feedModel)).Where(am => am != null));
                return articlelist;
            });
        }

        public override Task<bool> EvaluateArticle(ArticleModel articleModel)
        {
            return ExecuteSafe(async () =>
            {
                var article = await DownloadAsync(articleModel);
                article = article.Replace("[[]]", "[]");
                NzzArticle a = null;
                try
                {
                    a = JsonConvert.DeserializeObject<NzzArticle>(article);
                }
                catch (Exception)
                {
                    article = article.Replace(",\"publicationDateTime\":\"Invalid date\"", "");
                    try
                    {
                        a = JsonConvert.DeserializeObject<NzzArticle>(article);
                    }
                    catch
                    {
                        // no more fail safes
                    }
                }
                if (a != null)
                    return await ArticleToArticleModel(a, articleModel);
                return false;
            });
        }
    }
}
