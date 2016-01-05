using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources;
using OfflineMedia.Business.Sources.Bild;
using OfflineMedia.Business.Sources.Blick;
using OfflineMedia.Business.Sources.Nzz;
using OfflineMedia.Business.Sources.Postillon;
using OfflineMedia.Business.Sources.Spiegel;
using OfflineMedia.Business.Sources.Stern;
using OfflineMedia.Business.Sources.Tamedia;
using OfflineMedia.Business.Sources.Zeit;
using OfflineMedia.Business.Sources.ZwanzigMin;
using OfflineMedia.Common.Framework.Singleton;
using OfflineMedia.Common.Helpers;

namespace OfflineMedia.Business.Helpers
{
    public class ArticleHelper : SingletonBase<ArticleHelper>
    {
        public void OptimizeArticle(ref ArticleModel article)
        {
            if (article.Content == null || !article.Content.Any()) return;
            foreach (ContentModel cm in article.Content)
            {
                if (cm.Html != null)
                {
                    cm.Html = WebUtility.HtmlDecode(cm.Html);
                    cm.Html = cm.Html.Replace("&nbsp;", " ");
                }
            }
        }

        public void InformArticlesAboutTheirPosition(ObservableCollection<ArticleModel> list)
        {
            if (list != null && list.Count > 0)
            {
                for (int i = 1; i < list.Count - 1; i++)
                {
                    list[i].LeftArticle = list[i - 1];
                    list[i].RightArticle = list[i + 1];
                }
                if (list.Count > 2)
                    list[list.Count - 1].LeftArticle = list[list.Count - 2];
            }
        }

        public IMediaSourceHelper GetMediaSource(SourceEnum source)
        {
            IMediaSourceHelper sh = null;
            if (source == SourceEnum.Nzz)
            {
                sh = new NzzHelper();
            }
            else if (source == SourceEnum.Blick || source == SourceEnum.BlickAmAbend)
            {
                sh = new BlickHelper();
            }
            else if (source == SourceEnum.Postillon)
            {
                sh = new PostillonHelper();
            }
            else if ((int)source >= 20 && (int)source <= 40)
            {
                sh = new TamediaHelper();
            }
            else if (source == SourceEnum.ZwanzigMin)
            {
                sh = new ZwanzigMinHelper();
            }
            else if (source == SourceEnum.Stern)
            {
                sh = new SternHelper();
            }
            else if (source == SourceEnum.Spiegel)
            {
                sh = new SpiegelHelper();
            }
            else if (source == SourceEnum.Bild)
            {
                sh = new BildHelper();
            }
            else if (source == SourceEnum.Zeit)
            {
                sh = new ZeitHelper();
            }
            return sh;
        }

        public IMediaSourceHelper GetMediaSource(ArticleModel am)
        {
            if (am != null && am.FeedConfiguration != null && am.FeedConfiguration.SourceConfiguration != null)
                return GetMediaSource(am.FeedConfiguration.SourceConfiguration.Source);
            return null;
        }

        public List<string> GetKeywords(ArticleModel am)
        {
            IMediaSourceHelper sh = GetMediaSource(am);
            return sh != null ? sh.GetKeywords(am) : new List<string>();
        }

        public void AddWordDumpFromFeed(ref List<ArticleModel> am)
        {
            if (am != null && am.Any())
            {
                IMediaSourceHelper sh = GetMediaSource(am[0]);
                if (sh != null)
                {
                    foreach (var articleModel in am)
                    {
                        articleModel.WordDump = string.Join(" ", sh.GetKeywords(articleModel));
                    }
                }
            }
        }

        public void AddWordDumpFromArticle2(ref ArticleModel am)
        {
            if (am!= null && am.Content != null)
            {
                IMediaSourceHelper sh = GetMediaSource(am);
                if (sh != null)
                {
                    var countDic = new Dictionary<string, int>();
                    if (am.WordDump != null)
                    {
                        var words = am.WordDump.Split(' ');
                        foreach (var word in words)
                        {
                            countDic.Add(word, 2);
                        }
                    }

                    var text = "";
                    foreach (var contentModel in am.Content)
                    {
                        if (contentModel.ContentType == ContentType.Html)
                            text += contentModel.Html;
                    }
                    var keywords2 = TextHelper.Instance.GetImportantWords(text);

                    foreach (var id in keywords2)
                    {
                        if (countDic.ContainsKey(id))
                            countDic[id]++;
                        else
                            countDic.Add(id, 1);
                    }

                    var bestkeywords = countDic.OrderByDescending(d => d.Value).Select(d => d.Key).Take(10).ToList();
                    am.WordDump = string.Join(" ", bestkeywords);
                }
            }
        }
    }
}
