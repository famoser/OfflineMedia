using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Famoser.SqliteWrapper.Repositories;
using Famoser.SqliteWrapper.Services.Interfaces;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.ContentModels;
using OfflineMedia.Business.Newspapers;
using OfflineMedia.Business.Newspapers.Bild;
using OfflineMedia.Business.Newspapers.Blick;
using OfflineMedia.Business.Newspapers.Nzz;
using OfflineMedia.Business.Newspapers.Postillon;
using OfflineMedia.Business.Newspapers.Spiegel;
using OfflineMedia.Business.Newspapers.Stern;
using OfflineMedia.Business.Newspapers.Tamedia;
using OfflineMedia.Business.Newspapers.Welt;
using OfflineMedia.Business.Newspapers.Zeit;
using OfflineMedia.Business.Newspapers.ZwanzigMin;
using OfflineMedia.Data.Entities.Database.Contents;
using OfflineMedia.Data.Enums;
using OfflineMedia.Data.Helpers;

namespace OfflineMedia.Business.Helpers
{
    public class ArticleHelper
    {
        private static IMediaSourceHelper GetMediaSource(Sources source)
        {
            switch (source)
            {
                case Sources.Nzz:
                    return new NzzHelper();
                case Sources.Blick:
                case Sources.BlickAmAbend:
                    return new BlickHelper();
                case Sources.Postillon:
                    return new PostillonHelper();
                case Sources.ZwanzigMin:
                    return new ZwanzigMinHelper();
                case Sources.Stern:
                    return new SternHelper();
                case Sources.Spiegel:
                    return new SpiegelHelper();
                case Sources.Bild:
                    return new BildHelper();
                case Sources.Zeit:
                    return new ZeitHelper();
                case Sources.Welt:
                    return new WeltHelper();

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
                    return new TamediaHelper();
                case Sources.None:
                    return null;
                case Sources.Favorites:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public static IMediaSourceHelper GetMediaSource(ArticleModel am)
        {
            if (am.Feed?.Source != null)
                return GetMediaSource(am.Feed.Source.Source);
            return null;
        }

        public static IMediaSourceHelper GetMediaSource(FeedModel fm)
        {
            if (fm.Source != null)
                return GetMediaSource(fm.Source.Source);
            return null;
        }

        public static async Task SaveArticleLeadImage(ArticleModel model, ISqliteService service)
        {
            var imageContentGenericRepository = new GenericRepository<ImageContentModel, ImageContentEntity>(service);
            var oldLeadImages = (await service.GetByCondition<ContentEntity>(e => e.ParentId == model.GetId() && e.ContentType == (int)ContentType.LeadImage, null, false, 0, 0)).ToList();

            if (model.LeadImage != null)
            {
                if (model.LeadImage.GetId() != 0)
                {
                    var oldLeadImage = oldLeadImages.FirstOrDefault(o => o.ContentId == model.LeadImage.GetId());
                    if (oldLeadImage != null)
                        oldLeadImages.Remove(oldLeadImage);

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
            await service.DeleteAllById<ContentEntity>(oldLeadImages.Select(d => d.Id));
        }

        public static async Task SaveArticleContent(ArticleModel model, ISqliteService service)
        {
            var imageContentGenericRepository = new GenericRepository<ImageContentModel, ImageContentEntity>(service);
            var textContentGenericRepository = new GenericRepository<TextContentModel, TextContentEntity>(service);
            var galleryContentGenericRepository = new GenericRepository<GalleryContentModel, GalleryContentEntity>(service);

            var supportedContents = new[] { (int)ContentType.Text, (int)ContentType.Gallery, (int)ContentType.Image };
            var oldModels = (await service.GetByCondition<ContentEntity>(e => e.ParentId == model.GetId() && supportedContents.Any(s => s == e.ContentType), null, false, 0, 0)).ToList();
            for (int i = 0; i < model.Content.Count; i++)
            {
                var baseContentModel = model.Content[i];

                var entity = new ContentEntity();
                if (baseContentModel is TextContentModel)
                {
                    var text = (TextContentModel)baseContentModel;
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
                if (baseContentModel.GetId() == 0)
                {
                    await service.Add(entity);
                }
                else
                {
                    var oldmodel = oldModels.FirstOrDefault(m => m.ContentId == baseContentModel.GetId());
                    oldModels.Remove(oldmodel);
                }
            }
        }
    }
}
