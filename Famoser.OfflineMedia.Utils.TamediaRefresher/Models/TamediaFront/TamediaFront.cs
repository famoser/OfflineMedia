// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var data = TamediaFront.FromJson(jsonString);
//
// For POCOs visit quicktype.io?poco
//

using System;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Utils.TamediaRefresher.Models.TamediaFront
{
    public partial class TamediaFront
    {
        [JsonProperty("front")]
        public Front Front { get; set; }
    }

    public partial class Front
    {
        [JsonProperty("external_services")]
        public ExternalServices ExternalServices { get; set; }

        [JsonProperty("last_updated_at")]
        public long LastUpdatedAt { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("page_elements")]
        public PageElement[] PageElements { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }
    }

    public partial class ExternalServices
    {
        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("ads")]
        public Ads Ads { get; set; }

        [JsonProperty("paywall")]
        public Ads Paywall { get; set; }

        [JsonProperty("statistics")]
        public Statistic[] Statistics { get; set; }
    }

    public partial class Ads
    {
        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("main_channel")]
        public string MainChannel { get; set; }

        [JsonProperty("ad_unit")]
        public string AdUnit { get; set; }

        [JsonProperty("doc_type")]
        public string DocType { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }
    }

    public partial class Statistic
    {
        [JsonProperty("publish_date")]
        public long? PublishDate { get; set; }

        [JsonProperty("ipad_url")]
        public string IpadUrl { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("android_url")]
        public string AndroidUrl { get; set; }

        [JsonProperty("desktop_url")]
        public string DesktopUrl { get; set; }

        [JsonProperty("pagetype")]
        public string Pagetype { get; set; }

        [JsonProperty("iphone_url")]
        public string IphoneUrl { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("slideshow_title")]
        public string SlideshowTitle { get; set; }

        [JsonProperty("slideshow_id")]
        public long? SlideshowId { get; set; }

        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        [JsonProperty("video_id")]
        public long? VideoId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("video_title")]
        public string VideoTitle { get; set; }

        [JsonProperty("webapp_url")]
        public string WebappUrl { get; set; }
    }

    public partial class PageElement
    {
        [JsonProperty("category_article_count")]
        public long CategoryArticleCount { get; set; }

        [JsonProperty("linked_object_id")]
        public LinkedObjectId LinkedObjectId { get; set; }

        [JsonProperty("boxtype")]
        public string Boxtype { get; set; }

        [JsonProperty("articles")]
        public Articles[] Articles { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("external_services")]
        public OtherOtherOtherOtherExternalServices ExternalServices { get; set; }

        [JsonProperty("layout_type")]
        public string LayoutType { get; set; }

        [JsonProperty("linked_object_type")]
        public string LinkedObjectType { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("linked_object_placement_type")]
        public string LinkedObjectPlacementType { get; set; }

        [JsonProperty("meteonews")]
        public Meteonews Meteonews { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial struct LinkedObjectId
    {
        public long? Int;
        public string String;
    }

    public partial class Articles
    {
        [JsonProperty("genre")]
        public Decoration Genre { get; set; }

        [JsonProperty("main_category_for_site")]
        public CategoryForSite MainCategoryForSite { get; set; }

        [JsonProperty("category_for_site")]
        public CategoryForSite CategoryForSite { get; set; }

        [JsonProperty("author_annotation_prefix")]
        public string AuthorAnnotationPrefix { get; set; }

        [JsonProperty("allow_comments_weekends")]
        public bool AllowCommentsWeekends { get; set; }

        [JsonProperty("allow_comments_weekdays")]
        public bool AllowCommentsWeekdays { get; set; }

        [JsonProperty("article_elements")]
        public ArticleElements[] ArticleElements { get; set; }

        [JsonProperty("background_color")]
        public string BackgroundColor { get; set; }

        [JsonProperty("authors")]
        public Author[] Authors { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("external_services")]
        public OtherExternalServices ExternalServices { get; set; }

        [JsonProperty("comment_count_text")]
        public string CommentCountText { get; set; }

        [JsonProperty("comment_count")]
        public long CommentCount { get; set; }

        [JsonProperty("decoration")]
        public Decoration Decoration { get; set; }

        [JsonProperty("first_published_at")]
        public long FirstPublishedAt { get; set; }

        [JsonProperty("featured")]
        public bool Featured { get; set; }

        [JsonProperty("font_color")]
        public string FontColor { get; set; }

        [JsonProperty("layout_type_external_url")]
        public string LayoutTypeExternalUrl { get; set; }

        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("inline_elements")]
        public InlineElement[] InlineElements { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("is_rate_active")]
        public bool IsRateActive { get; set; }

        [JsonProperty("label_type")]
        public string LabelType { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("layout_type")]
        public string LayoutType { get; set; }

        [JsonProperty("lead_teaser")]
        public string LeadTeaser { get; set; }

        [JsonProperty("lead_eboard")]
        public string LeadEboard { get; set; }

        [JsonProperty("lead")]
        public string Lead { get; set; }

        [JsonProperty("lead_short")]
        public string LeadShort { get; set; }

        [JsonProperty("link_other")]
        public string LinkOther { get; set; }

        [JsonProperty("legacy_id")]
        public long LegacyId { get; set; }

        [JsonProperty("liveticker_id")]
        public string LivetickerId { get; set; }

        [JsonProperty("picture_wide_url")]
        public string PictureWideUrl { get; set; }

        [JsonProperty("picture_portrait_url")]
        public string PicturePortraitUrl { get; set; }

        [JsonProperty("picture_big_url")]
        public string PictureBigUrl { get; set; }

        [JsonProperty("mediatype")]
        public string Mediatype { get; set; }

        [JsonProperty("picture_medium_url")]
        public string PictureMediumUrl { get; set; }

        [JsonProperty("picture_square_url")]
        public string PictureSquareUrl { get; set; }

        [JsonProperty("picture_small_url")]
        public string PictureSmallUrl { get; set; }

        [JsonProperty("picture_wide_big_url")]
        public string PictureWideBigUrl { get; set; }

        [JsonProperty("source_annotation")]
        public string SourceAnnotation { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("share_count")]
        public long ShareCount { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("source_type")]
        public string SourceType { get; set; }

        [JsonProperty("timestamp_updated_at")]
        public long TimestampUpdatedAt { get; set; }

        [JsonProperty("title_short")]
        public string TitleShort { get; set; }

        [JsonProperty("title_teaser")]
        public string TitleTeaser { get; set; }

        [JsonProperty("title_eboard")]
        public string TitleEboard { get; set; }

        [JsonProperty("title_social_media")]
        public string TitleSocialMedia { get; set; }

        [JsonProperty("top_element")]
        public TopElement TopElement { get; set; }
    }

    public partial class Decoration
    {
        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }
    }

    public partial class CategoryForSite
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

    public partial class ArticleElements
    {
        [JsonProperty("iframe_height")]
        public long? IframeHeight { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("boxtype")]
        public string Boxtype { get; set; }

        [JsonProperty("article_previews")]
        public ArticlePreview[] ArticlePreviews { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("iframe_class_name")]
        public string IframeClassName { get; set; }

        [JsonProperty("picture_caption")]
        public string PictureCaption { get; set; }

        [JsonProperty("iframe_width")]
        public long? IframeWidth { get; set; }

        [JsonProperty("iframe_url")]
        public string IframeUrl { get; set; }

        [JsonProperty("lead")]
        public string Lead { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("picture_url")]
        public string PictureUrl { get; set; }

        [JsonProperty("tags")]
        public Author[] Tags { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class ArticlePreview
    {
        [JsonProperty("legacy_id")]
        public long LegacyId { get; set; }

        [JsonProperty("font_color")]
        public string FontColor { get; set; }

        [JsonProperty("decoration")]
        public Decoration Decoration { get; set; }

        [JsonProperty("background_color")]
        public string BackgroundColor { get; set; }

        [JsonProperty("authors")]
        public Author[] Authors { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("featured")]
        public bool Featured { get; set; }

        [JsonProperty("external_services")]
        public OtherExternalServices ExternalServices { get; set; }

        [JsonProperty("first_published_at")]
        public long FirstPublishedAt { get; set; }

        [JsonProperty("lead")]
        public string Lead { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("genre")]
        public Decoration Genre { get; set; }

        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("lead_short")]
        public string LeadShort { get; set; }

        [JsonProperty("lead_eboard")]
        public string LeadEboard { get; set; }

        [JsonProperty("lead_teaser")]
        public string LeadTeaser { get; set; }

        [JsonProperty("picture_wide_url")]
        public string PictureWideUrl { get; set; }

        [JsonProperty("picture_portrait_url")]
        public string PicturePortraitUrl { get; set; }

        [JsonProperty("picture_big_url")]
        public string PictureBigUrl { get; set; }

        [JsonProperty("mediatype")]
        public string Mediatype { get; set; }

        [JsonProperty("picture_medium_url")]
        public string PictureMediumUrl { get; set; }

        [JsonProperty("picture_square_url")]
        public string PictureSquareUrl { get; set; }

        [JsonProperty("picture_small_url")]
        public string PictureSmallUrl { get; set; }

        [JsonProperty("picture_wide_big_url")]
        public string PictureWideBigUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("timestamp_updated_at")]
        public long TimestampUpdatedAt { get; set; }

        [JsonProperty("title_short")]
        public string TitleShort { get; set; }

        [JsonProperty("title_eboard")]
        public string TitleEboard { get; set; }

        [JsonProperty("title_teaser")]
        public string TitleTeaser { get; set; }
    }

    public partial class Author
    {
        [JsonProperty("isin")]
        public string Isin { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("notation_number")]
        public long? NotationNumber { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("twitter")]
        public string Twitter { get; set; }

        [JsonProperty("rss")]
        public string Rss { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public partial class OtherExternalServices
    {
        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("ads")]
        public OtherAds Ads { get; set; }

        [JsonProperty("paywall")]
        public Paywall Paywall { get; set; }

        [JsonProperty("statistics")]
        public OtherStatistic[] Statistics { get; set; }
    }

    public partial class OtherAds
    {
        [JsonProperty("main_channel")]
        public string MainChannel { get; set; }

        [JsonProperty("article_id")]
        public long? ArticleId { get; set; }

        [JsonProperty("ad_unit")]
        public string AdUnit { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("show_ad")]
        public bool? ShowAd { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("sub_channel")]
        public string SubChannel { get; set; }
    }

    public partial class Paywall
    {
        [JsonProperty("cms_id")]
        public long CmsId { get; set; }

        [JsonProperty("main_channel")]
        public string MainChannel { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("doc_type")]
        public string DocType { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("sub_channel")]
        public string SubChannel { get; set; }
    }

    public partial class OtherStatistic
    {
        [JsonProperty("iphone_url")]
        public string IphoneUrl { get; set; }

        [JsonProperty("article_type")]
        public string ArticleType { get; set; }

        [JsonProperty("article_id")]
        public long? ArticleId { get; set; }

        [JsonProperty("android_url")]
        public string AndroidUrl { get; set; }

        [JsonProperty("article_title")]
        public string ArticleTitle { get; set; }

        [JsonProperty("desktop_url")]
        public string DesktopUrl { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("ipad_url")]
        public string IpadUrl { get; set; }

        [JsonProperty("publish_date")]
        public long? PublishDate { get; set; }

        [JsonProperty("premium")]
        public bool? Premium { get; set; }

        [JsonProperty("pagetype")]
        public string Pagetype { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("webapp_url")]
        public string WebappUrl { get; set; }
    }

    public partial class InlineElement
    {
        [JsonProperty("poll_id")]
        public string PollId { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("boxtype")]
        public string Boxtype { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("poll_legacy_id")]
        public long? PollLegacyId { get; set; }

        [JsonProperty("slideshow")]
        public Slideshow Slideshow { get; set; }

        [JsonProperty("video")]
        public Video Video { get; set; }
    }

    public partial class Slideshow
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("picture_big_url")]
        public string PictureBigUrl { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("article_preview")]
        public ArticlePreview ArticlePreview { get; set; }

        [JsonProperty("external_services")]
        public OtherOtherExternalServices ExternalServices { get; set; }

        [JsonProperty("links")]
        public Link[] Links { get; set; }

        [JsonProperty("lead")]
        public string Lead { get; set; }

        [JsonProperty("navigations")]
        public TamediaNavigation.Navigation[] Navigations { get; set; }

        [JsonProperty("picture_small_url")]
        public string PictureSmallUrl { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("picture_medium_url")]
        public string PictureMediumUrl { get; set; }

        [JsonProperty("pictures")]
        public Picture[] Pictures { get; set; }

        [JsonProperty("show_ad")]
        public bool ShowAd { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class OtherOtherExternalServices
    {
        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("ads")]
        public OtherAds Ads { get; set; }

        [JsonProperty("paywall")]
        public OtherPaywall Paywall { get; set; }

        [JsonProperty("statistics")]
        public Statistic[] Statistics { get; set; }
    }

    public partial class OtherPaywall
    {
        [JsonProperty("cms_id")]
        public long CmsId { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("doc_type")]
        public string DocType { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }
    }

    public partial class Link
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("target_blank")]
        public bool TargetBlank { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public partial class Navigation
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }
    }

    public partial class Picture
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("annotation_type")]
        public string AnnotationType { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("link_url")]
        public string LinkUrl { get; set; }

        [JsonProperty("link_title")]
        public string LinkTitle { get; set; }

        [JsonProperty("photographer")]
        public string Photographer { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }
    }

    public partial class Video
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("external_services")]
        public OtherOtherOtherExternalServices ExternalServices { get; set; }

        [JsonProperty("poster_picture_url")]
        public string PosterPictureUrl { get; set; }

        [JsonProperty("lead")]
        public string Lead { get; set; }

        [JsonProperty("published_date")]
        public long PublishedDate { get; set; }

        [JsonProperty("show_logo")]
        public bool ShowLogo { get; set; }

        [JsonProperty("show_ad")]
        public bool ShowAd { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public partial class OtherOtherOtherExternalServices
    {
        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("ads")]
        public OtherAds Ads { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("statistics")]
        public Statistic[] Statistics { get; set; }
    }

    public partial class TopElement
    {
        [JsonProperty("iframe_width")]
        public long? IframeWidth { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("boxtype")]
        public string Boxtype { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("iframe_height")]
        public long? IframeHeight { get; set; }

        [JsonProperty("iframe_class_name")]
        public string IframeClassName { get; set; }

        [JsonProperty("iframe_url")]
        public string IframeUrl { get; set; }

        [JsonProperty("picture_source_annotation")]
        public string PictureSourceAnnotation { get; set; }

        [JsonProperty("photographer")]
        public string Photographer { get; set; }

        [JsonProperty("lead")]
        public string Lead { get; set; }

        [JsonProperty("picture_caption")]
        public string PictureCaption { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("source_name")]
        public string SourceName { get; set; }

        [JsonProperty("picture_url")]
        public string PictureUrl { get; set; }

        [JsonProperty("slideshow")]
        public Slideshow Slideshow { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("video")]
        public Video Video { get; set; }
    }

    public partial class OtherOtherOtherOtherExternalServices
    {
        [JsonProperty("statistics")]
        public OtherOtherStatistic[] Statistics { get; set; }
    }

    public partial class OtherOtherStatistic
    {
        [JsonProperty("page_element")]
        public string PageElement { get; set; }

        [JsonProperty("element_position_offset")]
        public long ElementPositionOffset { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Meteonews
    {
        [JsonProperty("forecasts")]
        public Forecast[] Forecasts { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("geoname_id")]
        public long GeonameId { get; set; }

        [JsonProperty("observation")]
        public Observation Observation { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }
    }

    public partial class Forecast
    {
        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("end_timestamp")]
        public string EndTimestamp { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("temperature_avg")]
        public long TemperatureAvg { get; set; }

        [JsonProperty("symbol_code")]
        public long SymbolCode { get; set; }

        [JsonProperty("temperature_avg_unit")]
        public string TemperatureAvgUnit { get; set; }
    }

    public partial class Observation
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol_code")]
        public long SymbolCode { get; set; }

        [JsonProperty("cache")]
        public string Cache { get; set; }

        [JsonProperty("refresh")]
        public string Refresh { get; set; }

        [JsonProperty("temperature")]
        public long Temperature { get; set; }

        [JsonProperty("temperature_unit")]
        public string TemperatureUnit { get; set; }
    }


    public partial class TamediaFront
    {
        public static TamediaFront FromJson(string json)
        {
            return JsonConvert.DeserializeObject<TamediaFront>(json, Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this TamediaFront self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }

    public partial struct LinkedObjectId
    {
        public LinkedObjectId(JsonReader reader, JsonSerializer serializer)
        {
            Int = null;
            String = null;

            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    break;
                case JsonToken.Integer:
                    Int = serializer.Deserialize<long>(reader);
                    break;
                case JsonToken.String:
                case JsonToken.Date:
                    String = serializer.Deserialize<string>(reader);
                    break;
                default: throw new Exception("Cannot convert LinkedObjectId");
            }
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            if (Int != null)
            {
                serializer.Serialize(writer, Int);
                return;
            }
            if (String != null)
            {
                serializer.Serialize(writer, String);
                return;
            }
            writer.WriteNull();
        }
    }

    public class Converter : JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            return t == typeof(LinkedObjectId);
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (t == typeof(LinkedObjectId))
                return new LinkedObjectId(reader, serializer);
            throw new Exception("Unknown type");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = value.GetType();
            if (t == typeof(LinkedObjectId))
            {
                ((LinkedObjectId)value).WriteJson(writer, serializer);
                return;
            }
            throw new Exception("Unknown type");
        }

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = { new Converter() },
        };
    }
}
