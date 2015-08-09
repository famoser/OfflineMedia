using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Common.Framework.Singleton;

namespace OfflineMediaV3.Business.Helpers
{
    public class ArticleHelper : SingletonBase<ArticleHelper>
    {
        //public bool FusionNewOldArticlesV2(ref ObservableCollection<ArticleModel> target, ref List<ArticleModel> source)
        //{
        //    if (source == null)
        //        return true;

        //    try
        //    {
        //        var oldlist = target;
        //        while (target.Count > 0)
        //        {
        //            target.RemoveAt(0);
        //        }

        //        foreach (var articleModel in source)
        //        {
        //            var oldOne = oldlist.FirstOrDefault(o => o.PublicUri.AbsoluteUri == articleModel.PublicUri.AbsolutePath);
        //            target.Add(oldOne ?? articleModel);
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Instance.Log(LogLevel.Error, this, "Fusion of Lists failed V2", ex);
        //        return false;
        //    }
        //}

        public void OptimizeArticle(ref ArticleModel article)
        {
            if (article.Content == null || article.Content.Any()) return;
            foreach (ContentModel cm in article.Content)
            {
                if (cm.Html != null)
                {
                    cm.Html = WebUtility.HtmlDecode(cm.Html);
                }
            }
        }

        internal void InformArticlesAboutTheirPosition(ObservableCollection<ArticleModel> list)
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
    }
}
