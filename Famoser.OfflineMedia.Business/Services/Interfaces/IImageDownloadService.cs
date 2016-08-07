using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
