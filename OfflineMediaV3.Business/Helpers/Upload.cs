using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfflineMediaV3.Business.Framework.Communication;

namespace OfflineMediaV3.Business.Helpers
{
    public class Upload
    {
        public async Task<ServerResponse> UploadStats(ServerRequest postData, string url)
        {
            try
            {
                using (HttpClient wc = new HttpClient())
                {
                    HttpContent hc = new StringContent(JsonConvert.SerializeObject(postData));
                    var HtmlResult = await wc.PostAsync(url, hc);
                    string s = await HtmlResult.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServerResponse>(s);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
