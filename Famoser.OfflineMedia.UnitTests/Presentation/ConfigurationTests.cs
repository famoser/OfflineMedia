using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Famoser.OfflineMedia.Data.Entities.Storage.Settings;
using Famoser.OfflineMedia.Data.Entities.Storage.Sources;
using Famoser.OfflineMedia.Data.Enums;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.UnitTests.Presentation
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public async Task AllSettingsInConfiguration()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration/Settings.json"));
            var json = await FileIO.ReadTextAsync(file);
            var entities = JsonConvert.DeserializeObject<List<SettingEntity>>(json);
            var allKeys = Enum.GetValues(typeof (SettingKey));

            foreach (var allKey in allKeys)
            {
                var enu = (SettingKey) allKey;
                Assert.IsTrue(entities.Any(s => s.SettingKey == enu), "settingkey not defined: " + enu);
            }
        }

        [TestMethod]
        public async Task JsonCorrectlyMinified()
        {
            StorageFile file1 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration/Sources.json"));
            var json1 = await FileIO.ReadTextAsync(file1);
            var entities1 = JsonConvert.DeserializeObject<List<SourceEntity>>(json1);

            StorageFile file2 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration/Sources_min.json"));
            var json2 = await FileIO.ReadTextAsync(file2);
            var entities2 = JsonConvert.DeserializeObject<List<SourceEntity>>(json2);

            Assert.IsTrue(entities1.Count == entities2.Count);
            for (int i = 0; i < entities1.Count; i++)
            {
                Assert.IsTrue(entities1[i].Abbreviation == entities2[i].Abbreviation);
                Assert.IsTrue(entities1[i].LogicBaseUrl == entities2[i].LogicBaseUrl);
                Assert.IsTrue(entities1[i].Name == entities2[i].Name);
                Assert.IsTrue(entities1[i].PublicBaseUrl == entities2[i].PublicBaseUrl);
                Assert.IsTrue(entities1[i].Source == entities2[i].Source);
                CheckFeedEntityListsEqual(entities1[i].Feeds, entities2[i].Feeds);
            }

            StorageFile file3 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration/Settings.json"));
            var json3 = await FileIO.ReadTextAsync(file3);
            var entities3 = JsonConvert.DeserializeObject<List<SettingEntity>>(json3);

            StorageFile file4 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration/Settings_min.json"));
            var json4 = await FileIO.ReadTextAsync(file4);
            var entities4 = JsonConvert.DeserializeObject<List<SettingEntity>>(json4);

            Assert.IsTrue(entities3.Count == entities4.Count);
            for (int i = 0; i < entities3.Count; i++)
            {
                Assert.IsTrue(entities3[i].IsImmutable == entities4[i].IsImmutable);
                Assert.IsTrue(entities3[i].Guid == entities4[i].Guid);
                Assert.IsTrue(entities3[i].Name == entities4[i].Name);
                Assert.IsTrue(entities3[i].OffContent == entities4[i].OffContent);
                Assert.IsTrue(entities3[i].OnContent == entities4[i].OnContent);

                if (entities3[i].PossibleValues != null)
                {
                    Assert.IsTrue(entities4[i].PossibleValues != null);
                    Assert.IsTrue(entities3[i].PossibleValues.Length == entities4[i].PossibleValues.Length);
                    for (int j = 0; j < entities3[i].PossibleValues.Length; j++)
                        Assert.IsTrue(entities3[i].PossibleValues[j] == entities4[i].PossibleValues[j]);
                }
                else
                    Assert.IsTrue(entities4[i].PossibleValues == null);

                Assert.IsTrue(entities3[i].SettingKey == entities4[i].SettingKey);
                Assert.IsTrue(entities3[i].SettingValueType == entities4[i].SettingValueType);
                Assert.IsTrue(entities3[i].Value == entities4[i].Value);
            }
        }

        private void CheckFeedEntityListsEqual(List<FeedEntity> list1, List<FeedEntity> list2)
        {
            if (list1 == null && list2 == null)
                return;

            Assert.IsTrue(list1 != null && list2 != null);
            Assert.IsTrue(list1.Count == list2.Count);
            for (int i = 0; i < list1.Count; i++)
            {
                Assert.IsTrue(list1[i].Name == list2[i].Name);
                Assert.IsTrue(list1[i].Url == list2[i].Url);
                CheckFeedEntityListsEqual(list1[i].Feeds, list2[i].Feeds);
            }
        }

    }
}
