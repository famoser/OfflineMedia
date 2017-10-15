// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Famoser.OfflineMedia.Utils.TamediaRefresher.Models.TwentyMinSitemap;
//
//    var data = GettingStarted.FromJson(jsonString);
//
namespace Famoser.OfflineMedia.Utils.TamediaRefresher.Models.TwentyMinSitemap
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class GettingStarted
    {
        [JsonProperty("content")]
        public Content Content { get; set; }
    }

    public partial class Content
    {
        [JsonProperty("items")]
        public Items Items { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("jurisdiction")]
        public string Jurisdiction { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class Items
    {
        [JsonProperty("item")]
        public Item[] Item { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("feed_fullcontent_url")]
        public string FeedFullcontentUrl { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("adserver_url")]
        public string AdserverUrl { get; set; }

        [JsonProperty("category_id")]
        public string CategoryId { get; set; }

        [JsonProperty("enabled")]
        public string Enabled { get; set; }

        [JsonProperty("displayUrl")]
        public string DisplayUrl { get; set; }

        [JsonProperty("feed_full_content_url")]
        public string FeedFullContentUrl { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("front")]
        public long Front { get; set; }

        [JsonProperty("feed_preview_url")]
        public string FeedPreviewUrl { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("ids")]
        public string Ids { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public partial class GettingStarted
    {
        public static GettingStarted FromJson(string json) => JsonConvert.DeserializeObject<GettingStarted>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this GettingStarted self) => JsonConvert.SerializeObject(self, Converter.Settings);
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

