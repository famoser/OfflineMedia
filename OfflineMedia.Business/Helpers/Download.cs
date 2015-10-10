using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OfflineMedia.Common.Framework.Logs;

namespace OfflineMedia.Business.Helpers
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
                    if (url.ToString().Contains("api.sueddeutsche.de"))
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Signature keyId=\"3787d63b3f872f91\",algorithm=\"hmac - sha1\",headers=\"date x - nonce\",signature=\"MmQ3N2Q1YjRhNmE4YjRmZmUyMWY3MGMyZmQwODAyMTQ5Njk0OWFlOQ==\"");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("x-nonce", "534d0e50-fa50-4054-8947-c2e1312b0a1d;GT-N7100");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("date", "Tue, 01 09 2015 14:09:57 GMT+1000");
                    }
                    string s = await client.GetStringAsync(url);
                    return s;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "Download.cs", "DownloadStringAsync failed at 1 for url " + url, ex);
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
