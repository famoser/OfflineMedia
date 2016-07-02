using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Famoser.FrameworkEssentials.Singleton;
using Famoser.OfflineMedia.Data.Entities.Storage.Sources;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.UnitTests.Business.Newspapers.Helpers
{
    public class SourceTestHelper : SingletonBase<SourceTestHelper>
    {
        public async Task<List<SourceEntity>> GetSourceConfigs()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/SettingsUserConfiguration/Source.json"));
            var json = await FileIO.ReadTextAsync(file);
            return JsonConvert.DeserializeObject<List<SourceEntity>>(json);
        }
    }
}