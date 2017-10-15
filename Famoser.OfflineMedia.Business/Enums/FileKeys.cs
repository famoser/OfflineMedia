using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.OfflineMedia.Business.Enums
{
    public enum FileKeys
    {
        [Description("database.sqlite3")]
        Database,
        [Description("settings_configuration.V1")]
        SettingsUserConfiguration,
        [Description("sources_configuration.V1")]
        SourcesUserConfiguration,
        [Description("weather_cache.V1")]
        WeatherCache,
        [Description("Assets/Configuration/Settings.json")]
        SettingsConfiguration,
        [Description("Assets/Configuration/Sources.json")]
        SourcesConfiguration,
        [Description("Assets/Configuration/WeatherFontMapping.json")]
        WeatherFontInformations
    }
}
