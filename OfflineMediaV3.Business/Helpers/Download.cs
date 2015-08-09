using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Framework.Logs;

namespace OfflineMediaV3.Business.Helpers
{
    public class Download
    {
        public static async Task<string> DownloadStringAsync(Uri url)
        {
            try
            {
                using (var client = new HttpClient(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip
                                                 | DecompressionMethods.Deflate
                    }))
                {

                    string s = await client.GetStringAsync(url);
                    return s;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "Download.cs", "DownloadStringAsync failed at 1 for url " + url, ex);
            }

            //Trying again cause why the heck not
            try
            {
                using (var client = new HttpClient(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip
                                                 | DecompressionMethods.Deflate
                    }))
                {

                    string s = await client.GetStringAsync(url);
                    return s;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "Download.cs", "DownloadStringAsync failed at 2 for url "+url, ex);
            }
            return null;
        }

        public static async Task<Stream> DownloadStreamAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    return await client.GetStreamAsync(url);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "Download.cs", "DownloadStreamAsync failed for url " + url, ex);
            } 
            return null;
        }

        public static async Task<byte[]> DownloadImageAsync(Uri url)
        {
            if (url != null)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        return await client.GetByteArrayAsync(url);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, "Download.cs", "DownloadImageAsync failed for url " + url, ex);
                }
            }
            return null;
        }
    }
}
