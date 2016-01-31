using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Settings;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Services.Interfaces;
using OfflineMedia.Common.Framework.Timer;
using OfflineMedia.Data;
using OfflineMedia.Data.Entities;

namespace OfflineMedia.Business.Framework.Repositories
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

        public async Task<List<SourceConfigurationModel>> GetSourceConfigurations()
        {
            if (!_isInitialized)
                await Initialize();

            return _sourceConfigModels;
        }

        public async Task<List<SettingModel>> GetAllSettings()
        {
            if (!_isInitialized)
                await Initialize();

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
                    model.FeedConfigurationModels.Add(model2);
                    model2.SourceConfiguration = model;
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
                PublicBaseUrl = "PublicBaseUrl",
                FeedConfigurationModels = new List<FeedConfigurationModel>()
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

        private async Task<SettingModel> GetSettingByKey(SettingKeys key, IDataService dataService)
        {
            if (!_isInitialized)
                await Initialize();

            return _settingModels.FirstOrDefault(s => s.Key == key);
        }

        public async Task<SettingModel> GetSettingByKey(SettingKeys key)
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                return await GetSettingByKey(key, await unitOfWork.GetDataService());
            }
        }

        public async Task<bool> SaveSetting(SimpleSettingModel ssm)
        {
            try
            {
                if (_isInitialized)
                {
                    using (var unitOfWork = new UnitOfWork(false))
                    {
                        var repo = new GenericRepository<SimpleSettingModel, SettingEntity>(await unitOfWork.GetDataService());

                        await repo.Update(ssm);

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

        public async Task<bool> SaveSettingByKey(SettingKeys key, string value)
        {
            try
            {
                if (_isInitialized)
                {
                    using (var unitOfWork = new UnitOfWork(false))
                    {
                        var repo = new GenericRepository<SimpleSettingModel, SettingEntity>(await unitOfWork.GetDataService());

                        var set = await GetSettingByKey(key, await unitOfWork.GetDataService());
                        set.Value = value;

                        await repo.Update(set);

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

        public async Task<bool> SaveSettings()
        {
            try
            {
                if (_isInitialized)
                {
                    using (var unitOfWork = new UnitOfWork(false))
                    {
                        var repo = new GenericRepository<SimpleSettingModel, SettingEntity>(await unitOfWork.GetDataService());
                        var list = new List<SimpleSettingModel>();
                        list.AddRange(_settingModels);
                        list.AddRange(_sourceConfigModels);

                        foreach (var sourceConfigurationModel in _sourceConfigModels)
                        {
                            list.AddRange(sourceConfigurationModel.FeedConfigurationModels);
                        }

                        await repo.UpdateAll(list);

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

        public async Task<FeedConfigurationModel> GetFeedConfigurationFor(Guid guid)
        {
            if (!_isInitialized)
                await Initialize();

            return _sourceConfigModels.FirstOrDefault(s => s.FeedConfigurationModels.Any(d => d.Guid == guid)).FeedConfigurationModels.FirstOrDefault(d => d.Guid == guid);
        }

        private async Task InitializeTask(IDataService dataService)
        {
            try
            {
                TimerHelper.Instance.Stop("Loading JSON", this);
                var json = await _storageService.GetSettingsJson();
                _settingModels = JsonConvert.DeserializeObject<List<SettingModel>>(json);

                json = await _storageService.GetSourceJson();
                _sourceConfigModels = JsonConvert.DeserializeObject<List<SourceConfigurationModel>>(json);

                var repo = new GenericRepository<SimpleSettingModel, SettingEntity>(dataService);

                TimerHelper.Instance.Stop("Loading Settings", this);
                var guidSettings = await repo.GetByCondition(entity => true);

                TimerHelper.Instance.Stop("Sorting out Settings", this);
                //add new settings
                var newSettings = new List<SimpleSettingModel>();
                var enableSources = 0;
                var enableFeeds = 0;
                if (!guidSettings.Any())
                {
                    enableSources = 1;
                    enableFeeds = 3;
                }

                var sourceDic = new Dictionary<SimpleSettingModel, SourceConfigurationModel>();
                var feedDic = new Dictionary<SimpleSettingModel, FeedConfigurationModel>();
                foreach (var sourceConfigurationModel in _sourceConfigModels)
                {
                    var modl = guidSettings.FirstOrDefault(d => d.Guid == sourceConfigurationModel.Guid);
                    if (modl == null)
                    {
                        var ssm = new SimpleSettingModel
                        {
                            Guid = sourceConfigurationModel.Guid,
                            BoolValue = enableSources-- > 0
                        };
                        newSettings.Add(ssm);
                        sourceDic.Add(ssm, sourceConfigurationModel);
                    }
                    else
                    {
                        guidSettings.Remove(modl);
                        sourceDic.Add(modl, sourceConfigurationModel);
                    }

                    foreach (var feedConfigurationModel in sourceConfigurationModel.FeedConfigurationModels)
                    {
                        modl = guidSettings.FirstOrDefault(d => d.Guid == feedConfigurationModel.Guid);
                        if (modl == null)
                        {
                            var ssm = new SimpleSettingModel
                            {
                                Guid = feedConfigurationModel.Guid,
                                BoolValue = enableFeeds-- > 0
                            };
                            newSettings.Add(ssm);
                            feedDic.Add(ssm, feedConfigurationModel);
                        }
                        else
                        {
                            guidSettings.Remove(modl);
                            feedDic.Add(modl, feedConfigurationModel);
                        }

                        //relink
                        feedConfigurationModel.SourceConfiguration = sourceConfigurationModel;
                    }
                }

                var smDic = new Dictionary<SimpleSettingModel, SettingModel>();
                foreach (var settingModel in _settingModels)
                {
                    var modl = guidSettings.FirstOrDefault(d => d.Guid == settingModel.Guid);
                    if (modl == null)
                    {
                        var ssm = new SimpleSettingModel
                        {
                            Guid = settingModel.Guid,
                            Value = settingModel.DefaultValue
                        };
                        newSettings.Add(ssm);
                        smDic.Add(ssm, settingModel);
                    }
                    else
                    {
                        guidSettings.Remove(modl);
                        smDic.Add(modl, settingModel);
                    }
                }
                
                //write settings
                TimerHelper.Instance.Stop("Write Settings", this);

                //insert new settings
                if (newSettings.Count > 0)
                {
                    TimerHelper.Instance.Stop("Insert new Settings (" + newSettings.Count + ")", this);
                    await repo.AddAll(newSettings);
                }

                foreach (var sourceConfigurationModel in sourceDic)
                {
                    WriteProperties(sourceConfigurationModel.Value, sourceConfigurationModel.Key);
                }

                foreach (var feedConfigurationModel in feedDic)
                {
                    WriteProperties(feedConfigurationModel.Value, feedConfigurationModel.Key);
                }

                foreach (var settingModel in smDic)
                {
                    WriteProperties(settingModel.Value, settingModel.Key);
                }
                
                //remove old settings
                if (guidSettings.Count > 0)
                {
                    TimerHelper.Instance.Stop("Remove old Settings (" + guidSettings.Count + ")", this);
                    await repo.DeleteAll(guidSettings);
                }

                TimerHelper.Instance.Stop("Settings Initialized", this);

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "Settings could not be read out", ex);
            }
        }

        private Task _initializingTask;
        public async Task Initialize()
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                if (_initializingTask == null)
                    _initializingTask = InitializeTask(await unitOfWork.GetDataService());
            }
            if (!_initializingTask.IsCompleted)
                await _initializingTask;
        }

        private void WriteProperties(SettingModel to, SimpleSettingModel from)
        {
            to.Id = from.Id;
            to.ChangeDate = from.ChangeDate;
            to.CreateDate = from.CreateDate;
            to.Value = from.Value;
        }

        private void WriteProperties(FeedConfigurationModel to, SimpleSettingModel from)
        {
            to.Id = from.Id;
            to.ChangeDate = from.ChangeDate;
            to.CreateDate = from.CreateDate;
            to.Value = from.Value;
        }

        private void WriteProperties(SourceConfigurationModel to, SimpleSettingModel from)
        {
            to.Id = from.Id;
            to.ChangeDate = from.ChangeDate;
            to.CreateDate = from.CreateDate;
            to.Value = from.Value;
        }
    }
}
