using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Singleton;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using Famoser.OfflineMedia.Data.Entities.Database.Contents;
using Famoser.SqliteWrapper.Repositories;
using Famoser.SqliteWrapper.Services.Interfaces;

namespace Famoser.OfflineMedia.Business.Services
{
    public class ImageDownloadService : IImageDownloadService
    {
        private readonly IPlatformCodeService _platformCodeService;
        private readonly ISqliteService _sqliteService;
        private readonly GenericRepository<ImageContentModel, ImageContentEntity> _genericRepository;
        private readonly int _maxWidth;
        private readonly int _maxHeight;

        public ImageDownloadService(IPlatformCodeService platformCodeService, ISqliteService sqliteService)
        {
            _platformCodeService = platformCodeService;
            _sqliteService = sqliteService;

            _maxHeight = _platformCodeService.DeviceHeight();
            _maxWidth = _platformCodeService.DeviceWidth();

            _genericRepository = new GenericRepository<ImageContentModel, ImageContentEntity>(sqliteService);
        }

        private static readonly ConcurrentQueue<ImageContentModel> PriorityImages = new ConcurrentQueue<ImageContentModel>();
        private static readonly ConcurrentQueue<ImageContentModel> SecondaryImages = new ConcurrentQueue<ImageContentModel>();
        private static readonly ConcurrentBag<Task> ActiveTasks = new ConcurrentBag<Task>();

        private const int MaxActiveTasks = 5;

        public void Download(ImageContentModel imageContentModel, bool priority = false)
        {
            if (imageContentModel.LoadingState < LoadingState.Loaded)
                if (priority)
                {
                    if (!PriorityImages.Contains(imageContentModel))
                    {
                        PriorityImages.Enqueue(imageContentModel);
                        StartThreadIfNecessary();
                    }
                }
                else if (!SecondaryImages.Contains(imageContentModel))
                {
                    SecondaryImages.Enqueue(imageContentModel);
                    StartThreadIfNecessary();
                }
        }

        public void Download(FeedModel model)
        {
            for (int index = 0; index < model.AllArticles.Count; index++)
            {
                var articleModel = model.AllArticles[index];
                if (articleModel.LeadImage != null)
                    Download(articleModel.LeadImage, index < 15);
            }
        }

        public void Download(ArticleModel model)
        {
            if (model.LeadImage != null && model.LeadImage.LoadingState == LoadingState.New && !PriorityImages.Contains(model.LeadImage))
                Download(model.LeadImage, true);

            foreach (var baseContentModel in model.Content)
            {
                if (baseContentModel is ImageContentModel)
                {
                    Download((ImageContentModel)baseContentModel);
                }
                else if (baseContentModel is GalleryContentModel)
                {
                    foreach (var imageContentModel in ((GalleryContentModel)baseContentModel).Images)
                    {
                        Download(imageContentModel);
                    }
                }
            }
        }

        private void StartThreadIfNecessary()
        {
            if (PriorityImages.Any() || SecondaryImages.Any())
            {
                //might lead to too much tasks in concurrent setups, but does not really matter in this case
                for (int i = ActiveTasks.Count; i < MaxActiveTasks; i = ActiveTasks.Count)
                {
                    ActiveTasks.Add(DownloadImagesTask());
                }
            }
        }

        private async Task DownloadImagesTask()
        {
            try
            {
                ImageContentModel model;
                while (PriorityImages.TryDequeue(out model))
                {
                    try
                    {
                        var model1 = model;
                        _platformCodeService.CheckBeginInvokeOnUi(() =>
                        {
                            model1.LoadingState = LoadingState.Loading;
                        });

                        var img = await _platformCodeService.DownloadResizeImage(new Uri(model.Url), _maxHeight, _maxWidth).ConfigureAwait(false);

                        _platformCodeService.CheckBeginInvokeOnUi(() =>
                        {
                            model1.Image = img;
                            model1.LoadingState = LoadingState.Loaded;
                        }, async () => { await _genericRepository.SaveAsyc(model1); });
                    }
                    catch (Exception ex)
                    {
                        model.LoadingState = LoadingState.LoadingFailed;
                        await _genericRepository.SaveAsyc(model);
                        LogHelper.Instance.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
        }
    }
}
