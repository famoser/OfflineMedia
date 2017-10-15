// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Famoser.OfflineMedia.Business.Newspapers.ZwanzigMin.Models;
//
//    var data = GettingStarted.FromJson(jsonString);
//
namespace Famoser.OfflineMedia.Business.Newspapers.ZwanzigMin.Models
{
    using System;
    using Newtonsoft.Json;

    public partial class GettingStarted
    {
        [JsonProperty("content")]
        public Content Content { get; set; }
    }

    public partial class Category
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Content
    {
        [JsonProperty("jurisdiction")]
        public string Jurisdiction { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("adserver_url")]
        public string AdserverUrl { get; set; }

        [JsonProperty("items")]
        public Items Items { get; set; }

        [JsonProperty("nextpage")]
        public string Nextpage { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

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
        [JsonProperty("main_catid")]
        public string MainCatid { get; set; }

        [JsonProperty("disableMaxHeight")]
        public long? DisableMaxHeight { get; set; }

        [JsonProperty("catid")]
        public string Catid { get; set; }

        [JsonProperty("article_elements")]
        public ArticleElements[] ArticleElements { get; set; }

        [JsonProperty("adserver_url")]
        public string AdserverUrl { get; set; }

        [JsonProperty("actionicon")]
        public string Actionicon { get; set; }

        [JsonProperty("androidOverrideBodyAndDelayHeightCalculation")]
        public long? AndroidOverrideBodyAndDelayHeightCalculation { get; set; }

        [JsonProperty("autor_kuerzel")]
        public string AutorKuerzel { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("category_options")]
        public CategoryOptions CategoryOptions { get; set; }

        [JsonProperty("communityobject")]
        public Communityobject Communityobject { get; set; }

        [JsonProperty("channel_url")]
        public string ChannelUrl { get; set; }

        [JsonProperty("catname")]
        public string Catname { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("couchtype")]
        public string Couchtype { get; set; }

        [JsonProperty("context")]
        public object[] Context { get; set; }

        [JsonProperty("couchversion")]
        public long? Couchversion { get; set; }

        [JsonProperty("layoutid")]
        public string Layoutid { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("extraHTML")]
        public string ExtraHTML { get; set; }

        [JsonProperty("expDate")]
        public string ExpDate { get; set; }

        [JsonProperty("fixed")]
        public long? Fixed { get; set; }

        [JsonProperty("iosOverrideBodyAndDelayHeightCalculation")]
        public long? IosOverrideBodyAndDelayHeightCalculation { get; set; }

        [JsonProperty("images")]
        public Images[] Images { get; set; }

        [JsonProperty("layout_size")]
        public string LayoutSize { get; set; }

        [JsonProperty("livepage")]
        public string Livepage { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("lead")]
        public string Lead { get; set; }

        [JsonProperty("link_text")]
        public string LinkText { get; set; }

        [JsonProperty("livepage_url")]
        public string LivepageUrl { get; set; }

        [JsonProperty("livepage_nxp")]
        public long? LivepageNxp { get; set; }

        [JsonProperty("liveticker")]
        public string Liveticker { get; set; }

        [JsonProperty("pixel_ipad")]
        public string PixelIpad { get; set; }

        [JsonProperty("talkback_api_part_url")]
        public string TalkbackApiPartUrl { get; set; }

        [JsonProperty("picbig")]
        public string Picbig { get; set; }

        [JsonProperty("myvotebox")]
        public Myvotebox Myvotebox { get; set; }

        [JsonProperty("mobile_link")]
        public string MobileLink { get; set; }

        [JsonProperty("main_catname")]
        public string MainCatname { get; set; }

        [JsonProperty("multimedia_icons")]
        public MultimediaIcons MultimediaIcons { get; set; }

        [JsonProperty("pic_bigstory")]
        public string PicBigstory { get; set; }

        [JsonProperty("oberzeile")]
        public string Oberzeile { get; set; }

        [JsonProperty("pic_headline")]
        public string PicHeadline { get; set; }

        [JsonProperty("pixel_android_tablet")]
        public string PixelAndroidTablet { get; set; }

        [JsonProperty("picsmall")]
        public string Picsmall { get; set; }

        [JsonProperty("picmedium")]
        public string Picmedium { get; set; }

        [JsonProperty("pixel_android")]
        public string PixelAndroid { get; set; }

        [JsonProperty("pixel_bb")]
        public string PixelBb { get; set; }

        [JsonProperty("pixel_applewatch")]
        public string PixelApplewatch { get; set; }

        [JsonProperty("pixel_cablecom")]
        public string PixelCablecom { get; set; }

        [JsonProperty("qr_code")]
        public string QrCode { get; set; }

        [JsonProperty("pixel_win8_tablet")]
        public string PixelWin8Tablet { get; set; }

        [JsonProperty("pixel_samsung")]
        public string PixelSamsung { get; set; }

        [JsonProperty("pixel_iphone")]
        public string PixelIphone { get; set; }

        [JsonProperty("pixel_win8_phone")]
        public string PixelWin8Phone { get; set; }

        [JsonProperty("powered_by")]
        public PoweredBy PoweredBy { get; set; }

        [JsonProperty("pollid")]
        public Pollid Pollid { get; set; }

        [JsonProperty("pubDate")]
        public string PubDate { get; set; }

        [JsonProperty("show")]
        public Show Show { get; set; }

        [JsonProperty("sas_alias")]
        public object SasAlias { get; set; }

        [JsonProperty("revision")]
        public string Revision { get; set; }

        [JsonProperty("sas_extraHTML")]
        public string SasExtraHTML { get; set; }

        [JsonProperty("tags")]
        public Category[] Tags { get; set; }

        [JsonProperty("sourceid")]
        public Sourceid Sourceid { get; set; }

        [JsonProperty("tags_all")]
        public object[] TagsAll { get; set; }

        [JsonProperty("targetblank")]
        public string Targetblank { get; set; }

        [JsonProperty("talkback_popular_item")]
        public object[] TalkbackPopularItem { get; set; }

        [JsonProperty("talkback_count")]
        public string TalkbackCount { get; set; }

        [JsonProperty("talkback_api_url")]
        public string TalkbackApiUrl { get; set; }

        [JsonProperty("talkback_id")]
        public TalkbackId TalkbackId { get; set; }

        [JsonProperty("talkback_text")]
        public string TalkbackText { get; set; }

        [JsonProperty("talkback_status")]
        public string TalkbackStatus { get; set; }

        [JsonProperty("talkback_url")]
        public string TalkbackUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("teaser_ext")]
        public string TeaserExt { get; set; }

        [JsonProperty("teaser")]
        public Teaser Teaser { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("topelement")]
        public Topelement Topelement { get; set; }

        [JsonProperty("updDate")]
        public string UpdDate { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public partial class ArticleElements
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("comments_active")]
        public long? CommentsActive { get; set; }

        [JsonProperty("boxtype")]
        public string Boxtype { get; set; }

        [JsonProperty("comments_number")]
        public long? CommentsNumber { get; set; }

        [JsonProperty("thumbs_down")]
        public long? ThumbsDown { get; set; }

        [JsonProperty("shares_number")]
        public long? SharesNumber { get; set; }

        [JsonProperty("thumbs_up")]
        public long? ThumbsUp { get; set; }
    }

    public partial class CategoryOptions
    {
        [JsonProperty("radioplayer")]
        public long Radioplayer { get; set; }

        [JsonProperty("recommender")]
        public long Recommender { get; set; }
    }

    public partial class Communityobject
    {
        [JsonProperty("shares_sms")]
        public long? SharesSms { get; set; }

        [JsonProperty("share_url")]
        public string ShareUrl { get; set; }

        [JsonProperty("share_title")]
        public string ShareTitle { get; set; }

        [JsonProperty("couchtype")]
        public string Couchtype { get; set; }

        [JsonProperty("share_updated")]
        public long ShareUpdated { get; set; }

        [JsonProperty("shares_facebook")]
        public long SharesFacebook { get; set; }

        [JsonProperty("shares_email")]
        public long SharesEmail { get; set; }

        [JsonProperty("shares_other")]
        public long? SharesOther { get; set; }

        [JsonProperty("thumbs_down")]
        public long ThumbsDown { get; set; }

        [JsonProperty("shares_twitter")]
        public long? SharesTwitter { get; set; }

        [JsonProperty("shares_total")]
        public long SharesTotal { get; set; }

        [JsonProperty("shares_whatsapp")]
        public long SharesWhatsapp { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("thumbs_up")]
        public long ThumbsUp { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }

        [JsonProperty("vote_updated")]
        public long VoteUpdated { get; set; }
    }

    public partial class Images
    {
        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("orig_height")]
        public string OrigHeight { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("order")]
        public long Order { get; set; }

        [JsonProperty("orig_width")]
        public string OrigWidth { get; set; }

        [JsonProperty("orig_src")]
        public string OrigSrc { get; set; }

        [JsonProperty("photographer")]
        public string Photographer { get; set; }

        [JsonProperty("src_tablet_big")]
        public string SrcTabletBig { get; set; }

        [JsonProperty("src_medium")]
        public string SrcMedium { get; set; }

        [JsonProperty("src_big")]
        public string SrcBig { get; set; }

        [JsonProperty("src_small")]
        public string SrcSmall { get; set; }

        [JsonProperty("src_web_medium")]
        public string SrcWebMedium { get; set; }

        [JsonProperty("stampid")]
        public Stampid Stampid { get; set; }

        [JsonProperty("src_web_big")]
        public string SrcWebBig { get; set; }

        [JsonProperty("src_web_small")]
        public string SrcWebSmall { get; set; }

        [JsonProperty("stamppos")]
        public Stamppos Stamppos { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial struct Stampid
    {
        public long? Int;
        public string String;
    }

    public partial struct Stamppos
    {
        public long? Int;
        public string String;
    }

    public partial struct Myvotebox
    {
        public long? Int;
        public string String;
    }

    public partial class MultimediaIcons
    {
        [JsonProperty("video")]
        public long Video { get; set; }
    }

    public partial struct PoweredBy
    {
        public long? Int;
        public string String;
    }

    public partial struct Pollid
    {
        public long? Int;
        public string String;
    }

    public partial class Show
    {
        [JsonProperty("desktop")]
        public long Desktop { get; set; }

        [JsonProperty("app")]
        public long App { get; set; }

        [JsonProperty("mobile")]
        public long Mobile { get; set; }
    }

    public partial struct Sourceid
    {
        public long? Int;
        public string String;
    }

    public partial struct TalkbackId
    {
        public long? Int;
        public string String;
    }

    public partial struct Teaser
    {
        public long? Int;
        public string String;
    }

    public partial class Topelement
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class GettingStarted
    {
        public static GettingStarted FromJson(string json) =>
            JsonConvert.DeserializeObject<GettingStarted>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this GettingStarted self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public partial struct Stampid
    {
        public Stampid(JsonReader reader, JsonSerializer serializer)
        {
            Int = null;
            String = null;

            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    Int = serializer.Deserialize<long>(reader);
                    break;
                case JsonToken.String:
                case JsonToken.Date:
                    String = serializer.Deserialize<string>(reader);
                    break;
                default: throw new Exception("Cannot convert Stampid");
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
            throw new Exception("Union must not be null");
        }
    }

    public partial struct Stamppos
    {
        public Stamppos(JsonReader reader, JsonSerializer serializer)
        {
            Int = null;
            String = null;

            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    Int = serializer.Deserialize<long>(reader);
                    break;
                case JsonToken.String:
                case JsonToken.Date:
                    String = serializer.Deserialize<string>(reader);
                    break;
                default: throw new Exception("Cannot convert Stamppos");
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
            throw new Exception("Union must not be null");
        }
    }

    public partial struct Myvotebox
    {
        public Myvotebox(JsonReader reader, JsonSerializer serializer)
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
                default: throw new Exception("Cannot convert Myvotebox");
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

    public partial struct PoweredBy
    {
        public PoweredBy(JsonReader reader, JsonSerializer serializer)
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
                default: throw new Exception("Cannot convert PoweredBy");
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

    public partial struct Pollid
    {
        public Pollid(JsonReader reader, JsonSerializer serializer)
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
                default: throw new Exception("Cannot convert Pollid");
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

    public partial struct Sourceid
    {
        public Sourceid(JsonReader reader, JsonSerializer serializer)
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
                default: throw new Exception("Cannot convert Sourceid");
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

    public partial struct TalkbackId
    {
        public TalkbackId(JsonReader reader, JsonSerializer serializer)
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
                default: throw new Exception("Cannot convert TalkbackId");
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

    public partial struct Teaser
    {
        public Teaser(JsonReader reader, JsonSerializer serializer)
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
                default: throw new Exception("Cannot convert Teaser");
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
        public override bool CanConvert(Type t) => t == typeof(Myvotebox) || t == typeof(Pollid) ||
                                                   t == typeof(PoweredBy) || t == typeof(Sourceid) ||
                                                   t == typeof(Stampid) || t == typeof(Stamppos) ||
                                                   t == typeof(TalkbackId) || t == typeof(Teaser);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (t == typeof(Myvotebox))
                return new Myvotebox(reader, serializer);
            if (t == typeof(Pollid))
                return new Pollid(reader, serializer);
            if (t == typeof(PoweredBy))
                return new PoweredBy(reader, serializer);
            if (t == typeof(Sourceid))
                return new Sourceid(reader, serializer);
            if (t == typeof(Stampid))
                return new Stampid(reader, serializer);
            if (t == typeof(Stamppos))
                return new Stamppos(reader, serializer);
            if (t == typeof(TalkbackId))
                return new TalkbackId(reader, serializer);
            if (t == typeof(Teaser))
                return new Teaser(reader, serializer);
            throw new Exception("Unknown type");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = value.GetType();
            if (t == typeof(Myvotebox))
            {
                ((Myvotebox) value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(Pollid))
            {
                ((Pollid) value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(PoweredBy))
            {
                ((PoweredBy) value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(Sourceid))
            {
                ((Sourceid) value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(Stampid))
            {
                ((Stampid) value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(Stamppos))
            {
                ((Stamppos) value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(TalkbackId))
            {
                ((TalkbackId) value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(Teaser))
            {
                ((Teaser) value).WriteJson(writer, serializer);
                return;
            }
            throw new Exception("Unknown type");
        }

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {new Converter()},
        };
    }
}