using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Common.Framework.Singleton;

namespace OfflineMedia.Common.Framework.Timer
{
    public class TimerHelper : SingletonBase<TimerHelper>
    {
        private Tuple<DateTime, string> _lastEntry;
        private Tuple<DateTime, string> _firstEntry;
        private string _result;
        public void Stop(string description, object place)
        {
            var classname = place is string ? (string)place : place.GetType().Name;
            var newEntry =  new Tuple<DateTime, string>(DateTime.Now, classname + ": " + description);

            if (_lastEntry != null)
            {
                _result += classname + ": " + description + " " + FormatTimeSpan(DateTime.Now - _lastEntry.Item1) + " ms (" + FormatDateTime(DateTime.Now) + ")\n";
                _firstEntry = newEntry;
            }

            _lastEntry = newEntry;
        }

        public string GetAnalytics
        {
            get
            {
                if (_result == null)
                    return "No entries";
                return 
                    "Start: " + FormatDateTime(_firstEntry.Item1) + "\n" +
                    "End: " + FormatDateTime(_lastEntry.Item1) + "\n" +
                    "Duration: " + FormatTimeSpan(_lastEntry.Item1 - _firstEntry.Item1) + "\n" + "\n" + _result;
            }
        }

        private string FormatDateTime(DateTime date)
        {
            return date.ToString("hh:mm:ss.fff");
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            return (int)timeSpan.TotalMinutes + "m " + timeSpan.Seconds + "s " + timeSpan.Milliseconds + "ms";
        }
    }
}
