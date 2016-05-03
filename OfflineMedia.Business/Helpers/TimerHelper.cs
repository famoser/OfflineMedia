﻿using System;
using System.Collections.Generic;
using Famoser.FrameworkEssentials.Singleton;

namespace OfflineMedia.Business.Helpers
{
    public class TimerHelper : SingletonBase<TimerHelper>
    {
        private readonly Dictionary<Guid, Tuple<DateTime, string>> _lastEntry = new Dictionary<Guid, Tuple<DateTime, string>>();
        private readonly Dictionary<Guid, Tuple<DateTime, string>> _firstEntry = new Dictionary<Guid, Tuple<DateTime, string>>();
        private readonly Dictionary<Guid, string> _result = new Dictionary<Guid, string>();
        public void Stop(string description, object place, Guid? identi = null)
        {
#if DEBUG
            var classname = place is string ? (string)place : place.GetType().Name;
            var newEntry = new Tuple<DateTime, string>(DateTime.Now, classname + ": " + description);
            var identifier = identi.HasValue ? identi.Value : Guid.Empty;

            if (!_lastEntry.ContainsKey(identifier))
            {
                _lastEntry.Add(identifier, newEntry);
                _result.Add(identifier, classname + ": " + description + " (" + FormatDateTime(DateTime.Now) + ")\n");
                _firstEntry.Add(identifier, newEntry);
            }
            else
            {
                _result[identifier] += classname + ": " + description + " " + FormatTimeSpan(DateTime.Now - _lastEntry[identifier].Item1) + " (" + FormatDateTime(DateTime.Now) + ")\n";
                _lastEntry[identifier] = newEntry;
            }
#endif
        }

        public string GetAnalytics
        {
            get
            {
#if DEBUG
                var res = "";
                foreach (var s in _result)
                {
                    var key = "Key: " + s.Key + "\n";

                    if (_result.Values == null)
                        res += key + "No entries\n\n\n";
                    else
                        res += "Start: " + FormatDateTime(_firstEntry[s.Key].Item1) + "\n" +
                        "End: " + FormatDateTime(_lastEntry[s.Key].Item1) + "\n" +
                        "Duration: " + FormatTimeSpan(_lastEntry[s.Key].Item1 - _firstEntry[s.Key].Item1) + "\n" + "\n" + _result[s.Key] + "\n\n\n";
                }
                return res;
#else
                return "";
#endif
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