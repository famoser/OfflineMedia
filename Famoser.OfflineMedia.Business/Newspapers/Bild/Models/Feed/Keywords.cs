using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Business.Newspapers.Bild.Models.Feed
{
    public class Keywords
    {
        [JsonProperty(PropertyName = "1")]
        public string Keyboard1 { get; set; }

        [JsonProperty(PropertyName = "2")]
        public string Keyboard2 { get; set; }

        [JsonProperty(PropertyName = "3")]
        public string Keyboard3 { get; set; }

        [JsonProperty(PropertyName = "4")]
        public string Keyboard4 { get; set; }

        [JsonProperty(PropertyName = "5")]
        public string Keyboard5 { get; set; }

        [JsonProperty(PropertyName = "6")]
        public string Keyboard6 { get; set; }

        [JsonProperty(PropertyName = "7")]
        public string Keyboard7 { get; set; }

        [JsonProperty(PropertyName = "8")]
        public string Keyboard8 { get; set; }
    }
}
