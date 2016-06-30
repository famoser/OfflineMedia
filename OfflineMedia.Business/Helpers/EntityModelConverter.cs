using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.SqliteWrapper.Helpers;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Data.Entities.Storage;
using OfflineMedia.Data.Enums;

namespace OfflineMedia.Business.Helpers
{
    public class EntityModelConverter
    {
        public static BaseSettingModel Convert(SettingEntity entity)
        {
            BaseSettingModel model = null;
            if (entity.SettingValueType == SettingValueType.Int)
                model = new IntSettingModel();
            else if (entity.SettingValueType == SettingValueType.Text)
                model = new TextSettingModel();
            else if (entity.SettingValueType == SettingValueType.Select)
                model = new SelectSettingModel()
                {
                    PossibleValues = entity.PossibleValues
                };
            else if (entity.SettingValueType == SettingValueType.TrueOrFalse)
                model = new TrueOrFalseSettingModel()
                {
                    OnContent = entity.OnContent,
                    OffContent = entity.OffContent
                };
            else
                return null;

            WriteBaseSettingModelValues(model, entity);
            return model;
        }

        private static void WriteBaseSettingModelValues(BaseSettingModel model, SettingEntity entity)
        {
            model.Name = entity.Name;
            model.Guid = entity.Guid;
            model.SettingKey = entity.SettingKey;
            model.Value = entity.Value;
        }

        public static SourceModel Convert(SourceEntity sourceEntity)
        {
            return new SourceModel()
            {
                Guid = sourceEntity.Guid,
                Name = sourceEntity.Name,
                Abbreviation = sourceEntity.Abbreviation,
                LogicBaseUrl = sourceEntity.LogicBaseUrl,
                PublicBaseUrl = sourceEntity.PublicBaseUrl
            };
        }

        public static FeedModel Convert(FeedEntity feedEntity, SourceModel source, bool isActive)
        {
            return new FeedModel()
            {
                Guid = feedEntity.Guid,
                Name = feedEntity.Name,
                Source = source,
                Url = feedEntity.Url
            };
        }
    }
}
