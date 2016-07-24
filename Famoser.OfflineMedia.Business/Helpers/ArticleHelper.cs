using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Newspapers;
using Famoser.OfflineMedia.Business.Newspapers.Bild;
using Famoser.OfflineMedia.Business.Newspapers.Blick;
using Famoser.OfflineMedia.Business.Newspapers.Nzz;
using Famoser.OfflineMedia.Business.Newspapers.Postillon;
using Famoser.OfflineMedia.Business.Newspapers.Spiegel;
using Famoser.OfflineMedia.Business.Newspapers.Stern;
using Famoser.OfflineMedia.Business.Newspapers.Tamedia;
using Famoser.OfflineMedia.Business.Newspapers.Welt;
using Famoser.OfflineMedia.Business.Newspapers.Zeit;
using Famoser.OfflineMedia.Business.Newspapers.ZwanzigMin;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Data.Entities.Database;
using Famoser.OfflineMedia.Data.Entities.Database.Contents;
using Famoser.OfflineMedia.Data.Enums;
using Famoser.SqliteWrapper.Repositories;
using Famoser.SqliteWrapper.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Business.Helpers
{
    public class ArticleHelper
    {
        public static IMediaSourceHelper GetMediaSource(Sources source, IThemeRepository themeRepository)
        {
            switch (source)
            {
                case Sources.Nzz:
                    return new NzzHelper(themeRepository);
                case Sources.Blick:
                case Sources.BlickAmAbend:
                    return new BlickHelper(themeRepository);
                case Sources.Postillon:
                    return new PostillonHelper(themeRepository);
                case Sources.ZwanzigMin:
                    return new ZwanzigMinHelper(themeRepository);
                case Sources.Stern:
                    return new SternHelper(themeRepository);
                case Sources.Spiegel:
                    return new SpiegelHelper(themeRepository);
                case Sources.Bild:
                    return new BildHelper(themeRepository);
                case Sources.Zeit:
                    return new ZeitHelper(themeRepository);
                case Sources.Welt:
                    return new WeltHelper(themeRepository);

                case Sources.BaslerZeitung:
                case Sources.BernerZeitung:
                case Sources.DerBund:
                case Sources.Tagesanzeiger:
                case Sources.LeMatin:
                case Sources.VingtQuatreHeures:
                case Sources.TribuneDeGeneve:
                case Sources.ZuerichseeZeitung:
                case Sources.Landbote:
                case Sources.ZuericherUnterlaender:
                case Sources.BernerOeberlaender:
                case Sources.ThunerTagblatt:
                case Sources.LangenthalerTagblatt:
                    return new TamediaHelper(themeRepository);
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public static IMediaSourceHelper GetMediaSource(ArticleModel am, IThemeRepository themeRepository)
        {
            if (am.Feed?.Source != null)
                return GetMediaSource(am.Feed.Source.Source, themeRepository);
            return null;
        }

        public static IMediaSourceHelper GetMediaSource(FeedModel fm, IThemeRepository themeRepository)
        {
            if (fm.Source != null)
                return GetMediaSource(fm.Source.Source, themeRepository);
            return null;
        }

        public static async Task SaveArticle(ArticleModel model, ISqliteService service)
        {
            CleanUp(model);
            var articleGenericRepository = new GenericRepository<ArticleModel, ArticleEntity>(service);
            await articleGenericRepository.SaveAsyc(model);
            if (model.AfterSaveFunc != null)
            {
                var func = model.AfterSaveFunc;
                model.AfterSaveFunc = null;
                await func();
            }
        }

        public static async Task SaveArticleLeadImage(ArticleModel model, ISqliteService service, bool skipCleaning = false)
        {
            var imageContentGenericRepository = new GenericRepository<ImageContentModel, ImageContentEntity>(service);
            List<ContentEntity> oldLeadImages = null;
            var articleId = model.GetId();
            if (!skipCleaning)
                oldLeadImages = (await service.GetByCondition<ContentEntity>(e => e.ParentId == articleId && e.ContentType == (int)ContentType.LeadImage, null, false, 0, 0)).ToList();

            if (model.LeadImage != null)
            {
                if (model.LeadImage.GetId() != 0)
                {
                    var leadImageId = model.LeadImage.GetId();
                    var oldLeadImage = oldLeadImages?.FirstOrDefault(o => o.ContentId == leadImageId);
                    if (oldLeadImage != null)
                        oldLeadImages?.Remove(oldLeadImage);

                    await imageContentGenericRepository.SaveAsyc(model.LeadImage);
                }
                else
                {
                    await imageContentGenericRepository.SaveAsyc(model.LeadImage);

                    var entity = new ContentEntity
                    {
                        ContentId = model.LeadImage.GetId(),
                        ParentId = model.GetId(),
                        Index = 0,
                        ContentType = (int)ContentType.LeadImage
                    };
                    await service.Add(entity);
                }
            }

            if (!skipCleaning)
                await service.DeleteAllById<ContentEntity>(oldLeadImages.Select(d => d.Id));
        }

        public static async Task SaveArticleContent(ArticleModel model, ISqliteService service, bool skipCleaning = false)
        {
            var imageContentGenericRepository = new GenericRepository<ImageContentModel, ImageContentEntity>(service);
            var textContentGenericRepository = new GenericRepository<TextContentModel, TextContentEntity>(service);
            var galleryContentGenericRepository = new GenericRepository<GalleryContentModel, GalleryContentEntity>(service);

            var supportedContents = new[] { (int)ContentType.Text, (int)ContentType.Gallery, (int)ContentType.Image };
            List<ContentEntity> oldModels = null;
            if (!skipCleaning)
            {
                var id = model.GetId();
                oldModels = (await service.GetByCondition<ContentEntity>(e => e.ParentId == id, null,false, 0, 0)).ToList();
                oldModels = oldModels.Where(e => supportedContents.Any(s => s == e.ContentType)).ToList();
            }
            for (int i = 0; i < model.Content.Count; i++)
            {
                var baseContentModel = model.Content[i];

                ContentEntity entity = null;
                if (!skipCleaning)
                {
                    entity = oldModels.FirstOrDefault(m => m.ContentId == baseContentModel.GetId());
                    oldModels.Remove(entity);
                }

                if (entity == null)
                    entity = new ContentEntity();

                if (baseContentModel is TextContentModel)
                {
                    var text = (TextContentModel)baseContentModel;
                    text.ContentJson = JsonConvert.SerializeObject(text.Content);
                    await textContentGenericRepository.SaveAsyc(text);
                    entity.ContentType = (int)ContentType.Text;
                }
                else if (baseContentModel is ImageContentModel)
                {
                    var image = (ImageContentModel)baseContentModel;
                    if (image.Text != null)
                    {
                        await textContentGenericRepository.SaveAsyc(image.Text);
                        image.TextContentId = image.Text.GetId();
                    }
                    await imageContentGenericRepository.SaveAsyc(image);
                    entity.ContentType = (int)ContentType.Image;
                }
                else if (baseContentModel is GalleryContentModel)
                {
                    var gallery = (GalleryContentModel)baseContentModel;
                    if (gallery.Text != null)
                    {
                        await textContentGenericRepository.SaveAsyc(gallery.Text);
                        gallery.TextContentId = gallery.Text.GetId();
                    }
                    await galleryContentGenericRepository.SaveAsyc(gallery);
                    for (int index = 0; index < gallery.Images.Count; index++)
                    {
                        gallery.Images[index].GalleryId = gallery.GetId();
                        gallery.Images[index].GalleryIndex = index;
                        if (gallery.Images[index].Text != null)
                        {
                            await textContentGenericRepository.SaveAsyc(gallery.Images[index].Text);
                            gallery.Images[index].TextContentId = gallery.Images[index].Text.GetId();
                        }
                        await imageContentGenericRepository.SaveAsyc(gallery.Images[index]);
                    }
                    entity.ContentType = (int)ContentType.Gallery;
                }
                else
                {
                    continue;
                }
                entity.ContentId = baseContentModel.GetId();
                entity.ParentId = model.GetId();
                entity.Index = i;
                if (entity.Id == 0)
                    await service.Add(entity);
                else
                    await service.Update(entity);
            }

            if (!skipCleaning && oldModels != null)
                foreach (var contentEntity in oldModels)
                {
                    await service.DeleteById<ContentEntity>(contentEntity.Id);
                }
        }

        public static async Task<ArticleModel> LoadForFeed(int id, ISqliteService sqliteService)
        {
            var arRepo = new GenericRepository<ArticleModel, ArticleEntity>(sqliteService);
            var imgRepo = new GenericRepository<ImageContentModel, ImageContentEntity>(sqliteService);
            
            var art = await arRepo.GetByIdAsync(id);
            var contents = await sqliteService.GetByCondition<ContentEntity>(s => s.ParentId == id && s.ContentType == (int)ContentType.LeadImage, s => s.Index, false, 1, 0);
            if (contents?.FirstOrDefault() != null)
            {
                var image = await imgRepo.GetByIdAsync(contents.FirstOrDefault().ContentId);
                art.LeadImage = image;
            }
            return art;
        }

        public static void CleanUp(ArticleModel model)
        {
            model.Title = TextHelper.NormalizeString(model.Title);
            model.SubTitle = TextHelper.NormalizeString(model.SubTitle);
            model.Teaser = TextHelper.NormalizeString(model.Teaser);
        }
    }
}
