﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Famoser.FrameworkEssentials.Singleton;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Newspapers;
using OfflineMedia.Business.Repositories.Interfaces;
using OfflineMedia.Fakes;

namespace OfflineMedia.SourceTests.Helpers
{
    public class SourceTestHelper : SingletonBase<SourceTestHelper>
    {
        public async Task<List<SourceConfigurationModel>> GetSourceConfigs()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/SettingsUserConfiguration/Source.json"));
            var json = await FileIO.ReadTextAsync(file);
            var sc = JsonConvert.DeserializeObject<List<SourceConfigurationModel>>(json);
            foreach (var sourceConfigurationModel in sc)
            {
                foreach (var feedConfigurationModel in sourceConfigurationModel.FeedConfigurationModels)
                {
                    feedConfigurationModel.SourceConfiguration = sourceConfigurationModel;
                }
            }
            return sc;
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

        public void PrepareTests()
        {
            SimpleIoc.Default.Register<IThemeRepository, FakeThemeRepository>();
        }
    }
}