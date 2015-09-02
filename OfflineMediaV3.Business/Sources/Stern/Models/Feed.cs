using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Sources.Stern.Models
{
    public class Image
    {
        public string src { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Entry2
    {
        public string type { get; set; }
        public string articleType { get; set; }
        public string contentId { get; set; }
        public string timestamp { get; set; }
        public string kicker { get; set; }
        public string headline { get; set; }
        public string teaser { get; set; }
        public string adKeywords { get; set; }
        public string adZone { get; set; }
        public List<Image> images { get; set; }
        public int? adSpaceId { get; set; }
    }

    public class Entry
    {
        public string type { get; set; }
        public string headline { get; set; }
        public List<Entry2> entries { get; set; }
    }

    public class SternFeed
    {
        public string module { get; set; }
        public string category { get; set; }
        public string lastModified { get; set; }
        public int topPositionId { get; set; }
        public int interstitialId { get; set; }
        public int bottomPositionId { get; set; }
        public List<Entry> entries { get; set; }
        public string keywords { get; set; }
    }
}
