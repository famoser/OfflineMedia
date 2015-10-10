using Newtonsoft.Json;

namespace OfflineMedia.Business.Sources.Blick.Models
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
        private string _stringauthor;
        [JsonProperty("author")]
        public object author
        {
            get
            {
                if (_author != null)
                    return _author;
                return _stringauthor;
            }
            set
            {
                if (value is articlefeeditem)
                {
                    _author = (articlefeeditem)value;
                }
                else
                {
                    _stringauthor = value as string;
                }
            }
        }

        public string firstName;


    }
}
