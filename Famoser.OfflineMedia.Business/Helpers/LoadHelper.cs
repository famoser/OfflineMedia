using System.Linq;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using Famoser.OfflineMedia.Data.Entities.Database;
using Famoser.OfflineMedia.Data.Entities.Database.Contents;
using Famoser.SqliteWrapper.Repositories;
using Famoser.SqliteWrapper.Services.Interfaces;

namespace Famoser.OfflineMedia.Business.Helpers
{
    public class LoadHelper
    {
        public static async Task<ArticleModel> LoadForFeed(int id, ISqliteService sqliteService, IImageDownloadService imageDownloadService)
        {
            var arRepo = new GenericRepository<ArticleModel, ArticleEntity>(sqliteService);
            var imgRepo = new GenericRepository<ImageContentModel, ImageContentEntity>(sqliteService);

            var art = await arRepo.GetByIdAsync(id);
            var contents = await sqliteService.GetByCondition<ContentEntity>(s => s.ParentId == id && s.ContentType == (int)ContentType.LeadImage, s => s.Index, false, 1, 0);
            if (contents?.FirstOrDefault() != null)
            {
                var image = await imgRepo.GetByIdAsync(contents.FirstOrDefault().ContentId);
                art.LeadImage = image;

                if (art.LeadImage?.LoadingState < LoadingState.Loaded)
                    imageDownloadService.Download(art);
            }
            return art;
        }
    }
}
