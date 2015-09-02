using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Common.DebugHelpers
{
    public class Stoppwatch
    {
        public Stoppwatch()
        {
            LastTime = DateTime.Now;
            TimeSpans = new List<TimeSpan>();
        }

        public DateTime LastTime { get; set; }
        public List<TimeSpan> TimeSpans { get; set; }

        public void Stop()
        {
            var now = DateTime.Now;
            var span = now - LastTime;

            TimeSpans.Add(span);

            LastTime = now;
        }

        public string Evaluate
        {
            get
            {
                var r = "";
                foreach (var timeSpan in TimeSpans)
                {
                    if (timeSpan.Seconds > 0)
                        r += timeSpan.Seconds + "s ";

                    r += timeSpan.Milliseconds + "ms\n";
                }
                return r;
            }
        }
    }
}
