using Famoser.FrameworkEssentials.Attributes;

namespace OfflineMedia.Business.Enums
{
    public enum FileKeys
    {
        [Description("database.sqlite3")]
        Database,
        [Description("configuration.V1")]
        UserConfiguration,
        [Description("Assets/UserConfiguration/Settings_min.json")]
        SettingsConfiguration,
        [Description("Assets/UserConfiguration/Sources_min.json")]
        SourcesConfiguration
    }
}
