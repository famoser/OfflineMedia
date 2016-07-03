using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.OfflineMedia.UnitTests.Helpers.Models
{
    public class LogEntry
    {
        public string Content { get; set; }
        public bool IsFaillure { get; set; }
        public DateTime DateTime { get; } = DateTime.Now;

        public List<LogEntry> LogEntries { get; } = new List<LogEntry>();

        public override string ToString()
        {
            var res = "";
            if (IsFaillure)
                res += "FAILLURE: ";
            else
                res += "          ";

            res += "[" + DateTime.ToString(CultureInfo.InvariantCulture) + "] ";
            res += Content;
            return res;
        }

        public List<string> OutputAll()
        {
            var resList = new List<string> { ToString() };

            foreach (var logEntry in LogEntries)
            {
                var temp = logEntry.OutputAll();
                for (int i = 0; i < temp.Count; i++)
                {
                    temp[i] = "    " + temp[i];
                }
                resList.AddRange(temp);
            }
            return resList;
        }

        public List<string> OutputFaillures()
        {
            var resList = new List<string>();
            foreach (var logEntry in LogEntries)
            {
                resList.AddRange(logEntry.OutputFaillures());
            }

            if (resList.Any() || IsFaillure)
                resList.Add(ToString());

            return resList;
        }
    }
}
