using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Framework.Communication;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Helpers;

namespace OfflineMediaV3.Business.Framework.Repositories
{
    public class ApiRepository : IApiRepository
    {
        private ISettingsRepository _settingsRepository;
        public ApiRepository(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public async Task UploadStats()
        {
            var config = await _settingsRepository.GetSourceConfigurations();

            //Contact Server for Stats
            var postData = new ServerRequest();
            postData.InstallationId = (await _settingsRepository.GetSettingByKey(SettingKeys.UniqueDeviceId)).Value;
            postData.Entries = new List<ServerRequestEntry>();
            foreach (var item in config)
            {
                postData.Entries.Add(new ServerRequestEntry() { Guid = item.Guid.ToString(), Value = item.IsEnabled });
                foreach (var feedModel in item.Feeds)
                {
                    postData.Entries.Add(new ServerRequestEntry()
                    {
                        Guid = feedModel.Guid.ToString(),
                        Value = feedModel.IsEnabled
                    });
                }
            }
            await Statistics.UploadStats(postData);
        }
    }
}
