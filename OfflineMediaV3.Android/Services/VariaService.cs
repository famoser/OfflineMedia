using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Net;
using OfflineMediaV3.Common.Framework.Services.Interfaces;

namespace OfflineMediaV3.Android.Services
{
    public class VariaService : IVariaService
    {
        public async Task<bool> OpenInBrowser(Uri url)
        {
            var uri = global::Android.Net.Uri.Parse(url.AbsoluteUri);
            var intent = new Intent(Intent.ActionView, uri);
            RuntimeObjects.Context.StartActivity(intent);
            return true;
        }
    }
}