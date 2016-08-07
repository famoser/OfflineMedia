using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;

namespace Famoser.OfflineMedia.Business.Services.Interfaces
{
    public interface IImageDownloadService
    {
        void Download(ImageContentModel imageContentModel, bool priority = false);
        void Download(FeedModel model);
        void Download(ArticleModel model);
    }
}
