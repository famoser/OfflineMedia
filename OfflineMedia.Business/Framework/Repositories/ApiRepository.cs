using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfflineMedia.Business.Enums.Settings;
using OfflineMedia.Business.Framework.Communication;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Common.Framework.Logs;
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
            try
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
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
        }
    }
}
