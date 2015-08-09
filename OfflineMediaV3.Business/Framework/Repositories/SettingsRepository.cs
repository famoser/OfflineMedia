using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Enums.Settings;
using OfflineMediaV3.Business.Framework.Generic;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.Data;
using OfflineMediaV3.Data.Entities;

namespace OfflineMediaV3.Business.Framework.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly IStorageService _storageService;

        private List<SettingModel> _settingModels;
        private List<SourceConfigurationModel> _sourceConfigModels;
        private bool _isInitialized;
        public SettingsRepository(IStorageService storageService)
        {
            _storageService = storageService;
        }

        #region sample

        public async Task<List<SourceConfigurationModel>> GetSourceConfigurations(IDataService dataService)
        {
            if (!_isInitialized)
                await Initialize(dataService);

            return _sourceConfigModels;
        }

        public async Task<List<SettingModel>> GetAllSettings(IDataService dataService)
        {
            if (!_isInitialized)
                await Initialize(dataService);

            return _settingModels;
        }

        public List<SettingModel> GetSampleSettings()
        {
            var settings = new List<SettingModel>();

            var example1 = GetSampleSettingBase();
            example1.ValueType = ValueTypeEnum.Free;
            example1.Value = "Free Value type Value";
            settings.Add(example1);

            var example2 = GetSampleSettingBase();
            example2.ValueType = ValueTypeEnum.Int;
            example2.Value = "1";
            settings.Add(example2);

            var example3 = GetSampleSettingBase();
            example3.ValueType = ValueTypeEnum.PossibleValues;
            example3.Value = "Possible value 1";
            settings.Add(example3);

            var example4 = GetSampleSettingBase();
            example4.ValueType = ValueTypeEnum.TrueOrFalse;
            example4.Value = "True";
            settings.Add(example4);

            var example5 = GetSampleSettingBase();
            example5.ValueType = ValueTypeEnum.TrueOrFalse;
            example5.Value = "False";
            settings.Add(example5);

            return settings;
        }

        public List<SourceConfigurationModel> GetSampleSourceConfiguration()
        {
            var list = new List<SourceConfigurationModel>();
            for (int i = 0; i < 3; i++)
            {
                var model = GetSampleSourceBase();
                for (int j = 0; j < 4; j++)
                {
                    var model2 = GetSampleFeedBase();
                    model.Feeds.Add(model2);
                }
                list.Add(model);
            }
            return list;
        }

        private SettingModel GetSampleSettingBase()
        {
            return new SettingModel()
            {
                Description = "Setting Description",
                DefaultValue = "Default Setting Value",
                Id = 2,
                IsChangeable = true,
                OffContent = "Off Content",
                OnContent = "OnContent",
                PossibleValues = new[] { "Possible value 1", "Possbile value 2", "Possible value 3" }
            };
        }

        private SourceConfigurationModel GetSampleSourceBase()
        {
            return new SourceConfigurationModel()
            {
                Guid = Guid.NewGuid(),
                BoolValue = true,
                SourceNameLong = "Long Source Name",
                SourceNameShort = "SourceName",
                Source = SourceEnum.BaslerZeitung,
                LogicBaseUrl = "LogicBaseUrl",
                PublicBaseUrl = "PublicBaseUrl"
            };
        }

        private FeedConfigurationModel GetSampleFeedBase()
        {
            return new FeedConfigurationModel()
            {
                Guid = Guid.NewGuid(),
                BoolValue = true,
                Name = "Feed Sample",
                Url = "url"
            };
        }

        #endregion

        public async Task<SettingModel> GetSettingByKey(SettingKeys key, IDataService dataService)
        {
            if (!_isInitialized)
                await Initialize(dataService);

            return _settingModels.FirstOrDefault(s => s.Key == key);
        }

        public async Task<bool> SaveSettings()
        {
            try
            {
                if (_isInitialized)
                {
                    using (var unitOfWork = new UnitOfWork(false))
                    {
                        var repo2 = new GenericRepository<SimpleSettingModel, SettingEntity>(await unitOfWork.GetDataService());

                        foreach (var settingModel in _settingModels)
                        {
                            await repo2.AddOrUpdate(settingModel);
                        }

                        foreach (var sourceConfigurationModel in _sourceConfigModels)
                        {
                            await repo2.AddOrUpdate(sourceConfigurationModel);
                            foreach (var feedConfigurationModel in sourceConfigurationModel.Feeds)
                            {
                                await repo2.AddOrUpdate(feedConfigurationModel);
                            }
                        }

                        await unitOfWork.Commit();
                        return true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "SaveConfiguration", ex);
            }
            return false;
        }

        public async Task<SourceConfigurationModel> GetSourceConfigurationFor(Guid guid, IDataService dataService)
        {
            if (!_isInitialized)
                await Initialize(dataService);

            return _sourceConfigModels.FirstOrDefault(s => s.Guid == guid);
        }

        public async Task Initialize(IDataService dataService)
        {
            try
            {
                var json = await _storageService.GetSettingsJson();
                _settingModels = JsonConvert.DeserializeObject<List<SettingModel>>(json);

                json = await _storageService.GetSourceJson();
                _sourceConfigModels = JsonConvert.DeserializeObject<List<SourceConfigurationModel>>(json);

                var repo = new GenericRepository<SimpleSettingModel, SettingEntity>(dataService);

                var guidSettings = await repo.GetByCondition(entity => true);

                var newSettings = new List<SimpleSettingModel>();
                for (int index = 0; index < _sourceConfigModels.Count; index++)
                {
                    var sourceConfigurationModel = _sourceConfigModels[index];
                    var modl = guidSettings.FirstOrDefault(d => d.Guid == sourceConfigurationModel.Guid);
                    if (modl != null)
                    {
                        WriteProperties(ref sourceConfigurationModel, ref modl);
                        guidSettings.Remove(modl);

                        for (int i = 0; i < sourceConfigurationModel.Feeds.Count; i++)
                        {
                            var feedConfigurationModel = sourceConfigurationModel.Feeds[i];
                            feedConfigurationModel.SourceGuid = sourceConfigurationModel.Guid;

                            modl = guidSettings.FirstOrDefault(d => d.Guid == feedConfigurationModel.Guid);
                            if (modl != null)
                            {
                                WriteProperties(ref feedConfigurationModel, ref modl);
                                guidSettings.Remove(modl);
                            }
                            else
                            {
                                newSettings.Add(new SimpleSettingModel
                                {
                                    Guid = feedConfigurationModel.Guid,
                                    BoolValue = false
                                });
                            }
                        }
                    }
                    else
                    {
                        sourceConfigurationModel.BoolValue = false;
                        newSettings.Add(new SimpleSettingModel()
                        {
                            Guid = sourceConfigurationModel.Guid,
                            BoolValue = false
                        });

                        for (int i = 0; i < sourceConfigurationModel.Feeds.Count; i++)
                        {
                            var feedConfigurationModel = sourceConfigurationModel.Feeds[i];
                            feedConfigurationModel.SourceGuid = sourceConfigurationModel.Guid;

                            modl = guidSettings.FirstOrDefault(d => d.Guid == feedConfigurationModel.Guid);
                            if (modl != null)
                            {
                                WriteProperties(ref feedConfigurationModel, ref modl);
                                guidSettings.Remove(modl);
                            }
                            else
                            {
                                newSettings.Add(new SimpleSettingModel()
                                {
                                    Guid = feedConfigurationModel.Guid,
                                    BoolValue = false
                                });
                            }
                        }
                    }
                }

                for (int index = 0; index < _settingModels.Count; index++)
                {
                    var settingModel = _settingModels[index];
                    var modl = guidSettings.FirstOrDefault(d => d.Guid == settingModel.Guid);
                    if (modl != null)
                    {
                        WriteProperties(ref settingModel, ref modl);
                        guidSettings.Remove(modl);
                    }
                    else
                    {
                        newSettings.Add(new SimpleSettingModel()
                        {
                            Guid = settingModel.Guid,
                            Value = settingModel.DefaultValue
                        });
                    }
                }

                if (newSettings.Any())
                {
                    foreach (var guidSettingModel in newSettings)
                    {
                        await repo.AddOrUpdate(guidSettingModel);
                    }

                    foreach (var guidSettingModel in guidSettings)
                    {
                        await repo.Delete(guidSettingModel);
                    }
                }

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Settings could not be read out", ex);
            }
        }

        private void WriteProperties(ref SettingModel to, ref SimpleSettingModel from)
        {
            to.Id = from.Id;
            to.ChangeDate = from.ChangeDate;
            to.CreateDate = from.CreateDate;
            to.Value = from.Value;
        }

        private void WriteProperties(ref FeedConfigurationModel to, ref SimpleSettingModel from)
        {
            to.Id = from.Id;
            to.ChangeDate = from.ChangeDate;
            to.CreateDate = from.CreateDate;
            to.Value = from.Value;
        }

        private void WriteProperties(ref SourceConfigurationModel to, ref SimpleSettingModel from)
        {
            to.Id = from.Id;
            to.ChangeDate = from.ChangeDate;
            to.CreateDate = from.CreateDate;
            to.Value = from.Value;
        }

        private void WriteProperties(ref SimpleSettingModel to, ref SimpleSettingModel from)
        {
            to.Id = from.Id;
            to.ChangeDate = from.ChangeDate;
            to.CreateDate = from.CreateDate;
            to.Value = from.Value;
        }
    }
}
