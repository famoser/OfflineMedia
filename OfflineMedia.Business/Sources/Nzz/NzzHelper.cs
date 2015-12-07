using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources.Nzz.Models;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Singleton;
using OfflineMedia.Common.Helpers;

namespace OfflineMedia.Business.Sources.Nzz
{
    public class NzzHelper : SingletonBase<NzzHelper>, IMediaSourceHelper
    {
#pragma warning disable 1998
        public async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scm, FeedConfigurationModel fcm)
#pragma warning restore 1998
        {
            var articlelist = new List<ArticleModel>();
            if (feed != null)
            {
                try
                {
                    var f = JsonConvert.DeserializeObject<NzzFeed>(feed);
                    articlelist.AddRange(f.articles.Select(am => FeedToArticleModel(am, scm)).Where(am => am != null));
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.EvaluateFeed failed", ex);
                }
            }
            return articlelist;
        }

        public ArticleModel FeedToArticleModel(NzzFeedArticle nfa, SourceConfigurationModel scm)
        {
            if (nfa == null) return null;

            try
            {
                var a = new ArticleModel
                {
                    PublicationTime = nfa.publicationDateTime,
                    Title = nfa.title,
                    SubTitle = nfa.subTitle
                };


                a.LeadImage = LeadImageToImage(nfa.leadImage);

                a.LogicUri = new Uri(scm.LogicBaseUrl + nfa.path.Substring(1));
                string guid = nfa.path.Substring(nfa.path.LastIndexOf("/") + 1);
                a.PublicUri = new Uri(scm.PublicBaseUrl + guid);

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.FeedToArticleModel failed", ex);
                return null;
            }
        }

        public bool NeedsToEvaluateArticle()
        {
            return true;
        }

        public async Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            if (article == null) return new Tuple<bool, ArticleModel>(false, am);

            try
            {
                article = article.Replace("[[]]", "[]");
                var a = JsonConvert.DeserializeObject<NzzArticle>(article);
                return await ArticleToArticleModel(a, am);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.EvaluateArticle failed", ex);
            }
            return new Tuple<bool, ArticleModel>(false, am);
        }

        public bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            original.Content = evaluatedArticle.Content;
            original.Author = evaluatedArticle.Author;
            original.Themes = evaluatedArticle.Themes;
            return true;
        }

        public List<string> GetKeywords(ArticleModel articleModel)
        {
            var part1 = TextHelper.Instance.GetImportantWords(articleModel.Title);
            var part2 = TextHelper.Instance.GetImportantWords(articleModel.SubTitle);

            return TextHelper.Instance.FusionLists(part1, part2);
        }

        public async Task<Tuple<bool, ArticleModel>> ArticleToArticleModel(NzzArticle na, ArticleModel am)
        {
            if (na != null)
            {
                try
                {
                    am.Content = new List<ContentModel>();
                    for (int i = 0; i < na.body.Length; i++)
                    {
                        if (na.body[i].style == "h4")
                            na.body[i].style = "h2";
                        if (na.body[i].style == "h3")
                            na.body[i].style = "h1";
                        string starttag = "<" + na.body[i].style + ">";
                        string endtag = "</" + na.body[i].style + ">";
                        am.Content.Add(new ContentModel() { ContentType = ContentType.Html, Html = starttag + na.body[i].text + endtag });
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
                        am.Teaser = na.leadText;

                    var repo = SimpleIoc.Default.GetInstance<IThemeRepository>();
                    am.Themes = await repo.GetThemeModelsFor(na.departments);
                    am.Themes.Add(await repo.GetThemeModelFor(am.FeedConfiguration.Name));

                    return new Tuple<bool, ArticleModel>(true, am);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.ArticleToArticleModel deserialization failed", ex);
                }
            }
            return new Tuple<bool, ArticleModel>(false, am);
        }

        private ImageModel LeadImageToImage(NzzLeadImage li)
        {
            if (li != null)
            {
                try
                {
                    var img = new ImageModel { Html = li.caption, Author = li.source };
                    string uri = li.path.Replace("%width%", "640").Replace("%height%", "360").Replace("%format%", "text");
                    img.Url = new Uri(uri);
                    return img;
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.LeadImageToImage deserialization failed", ex);
                    return null;
                }
            }
            return null;
        }
    }
}
