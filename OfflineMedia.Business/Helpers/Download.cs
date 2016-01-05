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

                    string s = await client.GetStringAsync(url);

                    if (url.ToString().Contains("spiegel.de"))
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Host", "www.spiegel.de");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "max-age=0");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("DNT", "1");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "de-DE");

                        var str2 = await client.GetStringAsync(url);
                        var str3 = "";
                        var str4 = "";
                        var str5 = "";

                        HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                        var stream = await myHttpWebRequest.GetResponseAsync();
                        using (var reader = new StreamReader(stream.GetResponseStream(), Encoding.GetEncoding("iso-8859-1")))
                        {
                            str3 = reader.ReadToEnd();
                        }


                        using (var client2 = new HttpClient())
                        {
                            str4 = await client2.GetStringAsync(url);
                            var stream2 = await client2.GetStreamAsync(url);
                            using (var reader = new StreamReader(stream2, Encoding.GetEncoding("iso-8859-1")))
                            {
                                str5 = reader.ReadToEnd();
                            }
                        }

                        var request = (HttpWebRequest)WebRequest.Create(url);
                        request.Method = "GET";

                        var response = await request.GetResponseAsync();

                        StringBuilder stringBuilder = new StringBuilder();
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                stringBuilder.Append(line);
                            }
                        }
                        var str6 = stringBuilder.ToString();




                        if (s != str2 || str2 != str3 || str3 != str4 || str4 !=  str5 || str5 != str6)
                        {
                            "stupids!".ToString();
                        }
                    }
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
