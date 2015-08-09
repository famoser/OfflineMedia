using Newtonsoft.Json;

namespace OfflineMediaV3.Business.Sources.Blick.Models
{
    public class articlefeeditem
    {
        public string type;
        public string section;
        public string src;
        public string txt;
        public img img;
        public articlefeeditem[] items;

        private articlefeeditem _author;
        [JsonProperty("author")]
        public object author
        {
            get
            {
                return _author;
            }
            set
            {
                if (value is articlefeeditem)
                {
                    _author = (articlefeeditem)value;
                }
            }
        }

        public string firstName;


    }
}
