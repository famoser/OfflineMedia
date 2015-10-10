using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfflineMedia.Business.Framework.Communication;

namespace OfflineMedia.Business.Helpers
{
    public class Statistics
    {
        public static async Task UploadStats(ServerRequest postData)
        {
            try
            {
                const string uri = "http://offlinemedia.florianalexandermoser.ch/stats.php";

                using (HttpClient wc = new HttpClient())
                {
                    string content = JsonConvert.SerializeObject(postData);
                    HttpContent hc = new StringContent(content, Encoding.UTF8, "application/json");
                    await wc.PostAsync(uri, hc);
                    return;
                }
            }
            catch
            {
                return;
            }
        }
    }
}
