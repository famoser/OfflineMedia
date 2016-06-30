using Famoser.FrameworkEssentials.Attributes;

namespace OfflineMedia.Business.Enums
{
    public enum FileKeys
    {
        [Description("database.sqlite3")]
        Database,
        [Description("settings_configuration.V1")]
        SettingsUserConfiguration,
        [Description("sources_configuration.V1")]
        SourcesUserConfiguration,
        [Description("Assets/Configuration/Settings_min.json")]
        SettingsConfiguration,
        [Description("Assets/Configuration/Sources_min.json")]
        SourcesConfiguration,
        [Description("Assets/Configuration/WeatherFontMapping.json")]
        WeatherFontInformations
    }
}
