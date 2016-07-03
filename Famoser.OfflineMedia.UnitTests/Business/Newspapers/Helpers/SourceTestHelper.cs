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
        public async Task<List<SourceEntity>> GetSourceConfigs()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration/Sources.json"));
            var json = await FileIO.ReadTextAsync(file);
            return JsonConvert.DeserializeObject<List<SourceEntity>>(json);
        }
        public async Task<List<SourceModel>> GetSourceConfigModels()
        {
            var res = new List<SourceModel>();
            var entities = await GetSourceConfigs();
            foreach (var sourceEntity in entities)
            {
                var source = EntityModelConverter.Convert(sourceEntity);
                foreach (var feedEntity in sourceEntity.Feeds)
                {
                    var feed = EntityModelConverter.Convert(feedEntity, source, true);
                    source.AllFeeds.Add(feed);
                    source.ActiveFeeds.Add(feed);
                }
                res.Add(source);
            }
            return res;
        }
    }
}