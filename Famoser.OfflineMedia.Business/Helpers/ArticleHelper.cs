using System;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
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
using Famoser.OfflineMedia.Data.Enums;

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
    }
}
