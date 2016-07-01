using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Repositories.Base;
using OfflineMedia.Business.Repositories.Interfaces;

namespace OfflineMedia.Business.Newspapers
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

        protected virtual async Task<string> DownloadAsync(Uri url)
        {
            var service = new HttpService();
            var response = await service.DownloadAsync(url);
            return await response.GetResponseAsStringAsync();
        }

        protected ArticleModel ConstructArticleModel(FeedModel feed)
        {
            return new ArticleModel()
            {
                DownloadDateTime = DateTime.Now,
                Feed = feed
            };
        }

        protected async Task AddThemesAsync(ArticleModel model, string[] themes)
        {
            if (model.GetId() == 0)
                return;
            foreach (var theme in themes)
            {
                await _themeRepository.AddThemeToArticleAsync(model, theme);
            }
            await _themeRepository.AddThemeToArticleAsync(model, model.Feed.Name);
        }
    }
}
