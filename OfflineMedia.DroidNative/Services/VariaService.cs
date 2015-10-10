using System;
using System.Threading.Tasks;
using Android.Content;
using OfflineMediaV3.Common.Framework.Services.Interfaces;

namespace OfflineMediaV3.DroidNative.Services
{
    public class VariaService : IVariaService
    {
#pragma warning disable 1998
        public async Task<bool> OpenInBrowser(Uri url)
#pragma warning restore 1998
        {
            var uri = Android.Net.Uri.Parse(url.AbsoluteUri);
            var intent = new Intent(Intent.ActionView, uri);
            RuntimeObjects.Context.StartActivity(intent);
            return true;
        }
    }
}