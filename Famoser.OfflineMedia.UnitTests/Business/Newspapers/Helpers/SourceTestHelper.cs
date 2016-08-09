using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Famoser.FrameworkEssentials.Singleton;
using Famoser.OfflineMedia.Business.Helpers;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Data.Entities.Storage.Sources;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.UnitTests.Business.Newspapers.Helpers
{
    public class SourceTestHelper : SingletonBase<SourceTestHelper>
    {
        public async Task<List<SourceModel>> GetSourceConfigs()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration/Sources.json"));
            var json = await FileIO.ReadTextAsync(file);
            return JsonConvert.DeserializeObject<List<SourceModel>>(json);
        }
        public async Task<List<SourceModel>> GetSourceConfigModels()
        {
            var res = await GetSourceConfigs();
            foreach (var source in res)
            {
                foreach (var feed in source.Feeds)
                {
                    source.ActiveFeeds.Add(feed);
                }
                res.Add(source);
            }
            return res;
        }
    }
}