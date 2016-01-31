using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfflineMedia.Business.Enums.Settings;
using OfflineMedia.Business.Framework.Communication;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Common.Framework.Services.Interfaces;

namespace OfflineMedia.Business.Framework.Repositories
{
    public class ApiRepository : IApiRepository
    {
        private ISettingsRepository _settingsRepository;
        private IApiService _apiService;
        public ApiRepository(ISettingsRepository settingsRepository, IApiService apiService)
        {
            _settingsRepository = settingsRepository;
            _apiService = apiService;
        }

        public async Task UploadStats()
        {
            var config = await _settingsRepository.GetSourceConfigurations();
            var dic = new Dictionary<string, string>();
            foreach (var sourceConfigurationModel in config.Where(s => s.BoolValue))
            {
                if (sourceConfigurationModel.BoolValue)
                {
                    var feeds = "";
                    foreach (var feedConfigurationModel in sourceConfigurationModel.FeedConfigurationModels.Where(f => f.BoolValue))
                    {
                        feeds += feedConfigurationModel.Name + ", ";
                    }
                    feeds = feeds.Substring(0, feeds.Length - 2);
                    dic.Add(sourceConfigurationModel.SourceNameShort, feeds);
                }
            }
            await _apiService.UploadStats(dic);
            
            /* own stats; disablöed because data is never evaluated
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
            }*/
        }
    }
}
