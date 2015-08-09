using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Framework.Singleton;

namespace OfflineMediaV3.Business.Framework.Logs
{
    public class LogHelper : SingletonBase<LogHelper>
    {
        private List<LogModel> _logs = new List<LogModel>();
        public void Log(LogLevel level, object from, string message, Exception ex = null)
        {
            var lm = new LogModel
            {
                LogLevel = level,
                Message = message,
                IsReported = false
            };

            if (from != null)
                lm.Location = from.GetType().Namespace + "." + from.GetType().Name;

            if (ex != null)
                lm.Message += ex.ToString();

            _logs.Add(lm);
        }

        public void Log(LogLevel level, string from, string message, Exception ex = null)
        {
            var lm = new LogModel
            {
                LogLevel = level,
                Message = message,
                IsReported = false
            };

            if (from != null)
                lm.Location = from;

            if (ex != null)
                lm.Message += ex.ToString();

            _logs.Add(lm);
        }

    }
}
