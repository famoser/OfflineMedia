using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Data.Entities.Storage.Sources;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Utils.Commons
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = File.ReadAllText("Assets/Sources.json");
            var obj = JsonConvert.DeserializeObject<List<SourceEntity>>(json);
            foreach (var sourceEntity in obj)
            {
                Console.WriteLine("- " + sourceEntity.Name);
            }
            Console.ReadKey();
        }
    }
}
