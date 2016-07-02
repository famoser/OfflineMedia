using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services;
using Newtonsoft.Json;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Newspapers.Bild.Models.Feed;

namespace Utils.BildLinkAggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            var tsk = Task.Run(async () =>
            {
                await Execute();
            });
            tsk.Wait();
        }
        

        private static async Task Execute()
        {
            var service = new HttpService();
            var response = await service.DownloadAsync(new Uri("http://json.bild.de/servlet/json/android/26324062,cnv=true,v=94.json"));
            var feed = JsonConvert.DeserializeObject<FeedRoot>(await response.GetResponseAsStringAsync());
            foreach (var childNode in feed.__childNodes__)
            {
                
            }
        }
    }
}
