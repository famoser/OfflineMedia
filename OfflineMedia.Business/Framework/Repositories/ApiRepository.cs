using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMedia.Business.Enums.Settings;
using OfflineMedia.Business.Framework.Communication;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;

namespace OfflineMedia.Business.Framework.Repositories
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
            using (var unitOfWork = new UnitOfWork(true))
            {
                var config = await _settingsRepository.GetSourceConfigurations(await unitOfWork.GetDataService());

                //Contact Server for Stats
                var postData = new ServerRequest();
                postData.InstallationId = (await _settingsRepository.GetSettingByKey(SettingKeys.UniqueDeviceId, await unitOfWork.GetDataService())).Value;
                postData.Entries = new List<ServerRequestEntry>();
                foreach (var item in config)
                {
                    postData.Entries.Add(new ServerRequestEntry() {Guid = item.Guid.ToString(), Value = item.BoolValue});
                    foreach (var feedModel in item.FeedConfigurationModels)
                    {
                        postData.Entries.Add(new ServerRequestEntry()
                        {
                            Guid = feedModel.Guid.ToString(),
                            Value = feedModel.BoolValue
                        });
                    }
                }
                await Statistics.UploadStats(postData);
            }
        }
    }
}
