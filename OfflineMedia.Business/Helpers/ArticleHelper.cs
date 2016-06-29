using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Models.NewsModel;
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
using OfflineMedia.Data.Enums;
using OfflineMedia.Data.Helpers;

namespace OfflineMedia.Business.Helpers
{
    public class ArticleHelper
    {
        public static IMediaSourceHelper GetMediaSource(Sources source)
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
    }
}
