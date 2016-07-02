using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Newspapers.Nzz.Models;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Business.Newspapers.Nzz
{
    public class NzzHelper : BaseMediaSourceHelper
    {
        public ArticleModel FeedToArticleModel(NzzFeedArticle nfa, FeedModel scm)
        {
            if (nfa == null) return null;

            try
            {
                var a = ConstructArticleModel(scm);
                a.PublishDateTime = nfa.publicationDateTime;
                a.Title = nfa.title;
                a.SubTitle = nfa.subTitle;

                a.LeadImage = LeadImageToImage(nfa.leadImage);

                a.LogicUri = scm.Source.LogicBaseUrl + nfa.path.Substring(1);
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
                    am.Content.Add(new TextContentModel()
                    {
                        Content = HtmlConverter.HtmlToParagraph(starttag + na.body[i].text + endtag)
                    });
                }

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
                    am.Author += na.agency;

                if (!string.IsNullOrEmpty(na.leadText))
                    am.Teaser = na.leadText.Replace(" \n", " ");

                await AddThemesAsync(am, na.departments);

                return true;
            });
        }

        private ImageContentModel LeadImageToImage(NzzLeadImage li)
        {
            if (li != null)
            {
                try
                {
                    var img = new ImageContentModel()
                    {
                        Text = new TextContentModel()
                        {
                            Content = HtmlConverter.HtmlToParagraph(li.caption)
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
                        var uri = li.path.Replace("%width%", "640")
                            .Replace("%height%", "360")
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
                var a = JsonConvert.DeserializeObject<NzzArticle>(article);
                return await ArticleToArticleModel(a, articleModel);
            });
        }
    }
}
