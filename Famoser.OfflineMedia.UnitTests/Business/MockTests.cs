using System;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Repositories.Mocks;
using Famoser.OfflineMedia.Data.Enums;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Famoser.OfflineMedia.UnitTests.Business
{
    [TestClass]
    public class MockTests
    {
        [TestMethod]
        public async Task SettingRepositoryAllSettingsMocked()
        {
            var repo = new SettingsRepositoryMock();
            var values = Enum.GetValues(typeof (SettingKey));

            foreach (var value in values)
            {
                var enu = (SettingKey) value;
                var setting = await repo.GetSettingByKeyAsync(enu);
                Assert.IsNotNull(setting, enu + " setting is null!");
            }
        }
    }
}
