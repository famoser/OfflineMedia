using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Sources.Bild.Models.Feed;

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
            var json = await Download.DownloadStringAsync(new Uri("http://json.bild.de/servlet/json/android/26324062,cnv=true,v=94.json"));
            var feed = JsonConvert.DeserializeObject<FeedRoot>(json);
            foreach (var childNode in feed.__childNodes__)
            {
                
            }
        }
    }
}
