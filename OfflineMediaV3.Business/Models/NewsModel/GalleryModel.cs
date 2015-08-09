using System.Collections.ObjectModel;
using OfflineMediaV3.Business.Framework;

namespace OfflineMediaV3.Business.Models.NewsModel
{
    public class GalleryModel : BaseModel
    {
        [EntityMap]
        public string Title { get; set; }

        [EntityMap]
        public string SubTitle { get; set; }

        [EntityMap]
        public string Html { get; set; }

        public ObservableCollection<ImageModel> Images { get; set; }
    }
}
