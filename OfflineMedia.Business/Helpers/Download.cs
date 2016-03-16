using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                    if (url.ToString().Contains("api.sueddeutsche.de"))
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Signature keyId=\"3787d63b3f872f91\",algorithm=\"hmac - sha1\",headers=\"date x - nonce\",signature=\"MmQ3N2Q1YjRhNmE4YjRmZmUyMWY3MGMyZmQwODAyMTQ5Njk0OWFlOQ==\"");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("x-nonce", "534d0e50-fa50-4054-8947-c2e1312b0a1d;GT-N7100");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("date", "Tue, 01 09 2015 14:09:57 GMT+1000");
                    }

                    //parse manually
                    if (url.ToString().Contains("xml.zeit.de"))
                    {
                        var stream2 = await client.GetStreamAsync(url);
                        using (var reader = new StreamReader(stream2, Encoding.GetEncoding("iso-8859-1")))
                        {
                            return reader.ReadToEnd();
                        }
                    }

                    return await client.GetStringAsync(url).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                var res = await MakeBaseRequest(url);
                if (res != null)
                {
                    LogHelper.Instance.Log(LogLevel.Info, "Download.cs", "DownloadStringAsync failed for url " + url + " but successfully recovered", ex);
                    return res;
                }
            }
            return null;
        }

        private static async Task<string> MakeBaseRequest(Uri url, Encoding enc = null)
        {
            try
            {
                if (enc == null)
                    enc = Encoding.GetEncoding("iso-8859-1");

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                var response = await request.GetResponseAsync().ConfigureAwait(false);

                StringBuilder stringBuilder = new StringBuilder();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), enc))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        stringBuilder.Append(line);
                    }
                }
                var str2 = stringBuilder.ToString();
                return str2;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Warning, "Download.cs", "MakeBaseRequest failed: " + url.AbsoluteUri, ex);
            }
            return null;
        }

        public static async Task<Stream> DownloadStreamAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    return await client.GetStreamAsync(url).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Warning, "Download.cs", "DownloadStreamAsync failed: " + url, ex);
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
                        client.Timeout = TimeSpan.FromSeconds(30);
                        var str = await client.GetStreamAsync(url);
                        return ReadFully(str);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Warning, "Download.cs", "DownloadImageAsync failed: " + url.AbsoluteUri, ex);
                }
            }
            return null;
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
