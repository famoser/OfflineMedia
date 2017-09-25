// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var data = TamediaNavigation.FromJson(jsonString);
//
// For POCOs visit quicktype.io?poco
//

using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Utils.TamediaRefresher.Models.TamediaNavigation
{
    public partial class TamediaNavigation
    {
        [JsonProperty("content")]
        public Content Content { get; set; }

        [JsonProperty("navigations")]
        public Navigation[] Navigations { get; set; }
    }

    public partial class Content
    {
        [JsonProperty("jurisdiction")]
        public string Jurisdiction { get; set; }
    }

    public partial class Navigation
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("category_preview")]
        public CategoryPreview CategoryPreview { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("navigation_type")]
        public string NavigationType { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("sub_navigations")]
        public SubNavigation[] SubNavigations { get; set; }
    }

    public partial class CategoryPreview
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class SubNavigation
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("navigation_type")]
        public string NavigationType { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }
    }


    public partial class TamediaNavigation
    {
        public static TamediaNavigation FromJson(string json)
        {
            return JsonConvert.DeserializeObject<TamediaNavigation>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this TamediaNavigation self)
        {
            return JsonConvert.SerializeObject((object) self, (JsonSerializerSettings) Converter.Settings);
        }
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
