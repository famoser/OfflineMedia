using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using OfflineMediaV3.Business.Helpers;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Sources;
using OfflineMediaV3.Common.Framework.Singleton;

namespace OfflineMediaV3.UnitTestApp.SourceTests.Helpers
{
    public class SourceTestHelper : SingletonBase<SourceTestHelper>
    {
        public async Task<List<SourceConfigurationModel>> GetSourceConfigs()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration/Source.json"));
            var json = await FileIO.ReadTextAsync(file);
            return JsonConvert.DeserializeObject<List<SourceConfigurationModel>>(json);
        }

        public async Task<List<ArticleModel>> GetFeedFor(IMediaSourceHelper mediaSourceHelper, SourceConfigurationModel sourceConfigModel, FeedConfigurationModel feedConfigModel)
        {
            string feedresult = await Download.DownloadStringAsync(new Uri(feedConfigModel.Url));
            var res = await mediaSourceHelper.EvaluateFeed(feedresult, sourceConfigModel, feedConfigModel);
            foreach (var article in res)
            {
                article.FeedConfiguration = feedConfigModel;
            }
            return res;
        }
    }
}