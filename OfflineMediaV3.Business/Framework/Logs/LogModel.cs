using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Framework.Logs
{
    public class LogModel
    {
        public LogLevel LogLevel { get; set; }

        public string Location { get; set; }
        public string Message { get; set; }

        public bool IsReported { get; set; }
    }
}
