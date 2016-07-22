using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Base;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;

namespace Famoser.OfflineMedia.Business.Newspapers
{
    public abstract class BaseMediaSourceHelper : BaseRepository, IMediaSourceHelper
    {
        private IThemeRepository _themeRepository;

        protected BaseMediaSourceHelper(IThemeRepository themeRepository)
        {
            _themeRepository = themeRepository;
        }

        public abstract Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel);

        public abstract Task<bool> EvaluateArticle(ArticleModel articleModel);

        protected Task<string> DownloadAsync(ArticleModel model)
        {
            return DownloadAsync(new Uri(model.LogicUri));
        }

        protected Task<string> DownloadAsync(FeedModel model)
        {
            return DownloadAsync(model.GetLogicUri());
        }

        private HttpService _httpServiceCache;
        protected virtual async Task<string> DownloadAsync(Uri url)
        {
            if (_httpServiceCache == null)
                _httpServiceCache = new HttpService();
            var response = await _httpServiceCache.DownloadAsync(url);
            if (response != null)
                return await response.GetResponseAsStringAsync();
            return null;
        }

        protected ArticleModel ConstructArticleModel(FeedModel feed)
        {
            return new ArticleModel()
            {
                DownloadDateTime = DateTime.Now,
                Feed = feed
            };
        }

        protected async Task AddThemesAsync(ArticleModel model, string[] themes = null)
        {
            if (model.GetId() == 0)
                return;
            if (themes != null)
                foreach (var theme in themes.Where(t => t != null))
                {
                    await _themeRepository.AddThemeToArticleAsync(model, theme);
                }

            await _themeRepository.AddThemeToArticleAsync(model, model.Feed.Name);
        }
    }
}
