﻿using System;
using System.Threading.Tasks;

namespace Famoser.OfflineMedia.Business.Services
{
    public interface IPlatformCodeService
    {
        Task<byte[]> DownloadResizeImage(Uri url);
        void CheckBeginInvokeOnUi(Action action);
        Task<bool> OpenInBrowser(Uri url);
    }
}
