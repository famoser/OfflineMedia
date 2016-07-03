using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.UnitTests.Helpers.Models;

namespace Famoser.OfflineMedia.UnitTests.Helpers
{
    public class Logger : IDisposable
    {
        private string _identifier;
        public Logger(string identifier)
        {
            _identifier = identifier;
        }

        private readonly List<LogEntry> _logs = new List<LogEntry>(); 
        public void Log(string content, bool isFaillure = false)
        {
            _logs.Add(new LogEntry()
            {
                Content = content,
                IsFaillure = isFaillure
            });
        }

        public void AddLog(LogEntry entry)
        {
            _logs.Add(entry);
        }

        public bool HasEntryWithFaillure()
        {
            return _logs.Any(logEntry => logEntry.OutputFaillures().Any());
        }

        public void Dispose()
        {
            var logs = new List<string>();
            foreach (var logEntry in _logs)
            {
                logs.AddRange(logEntry.OutputAll());
            }
            var faillures = new List<string>();
            foreach (var logEntry in _logs)
            {
                faillures.AddRange(logEntry.OutputFaillures());
            }

            File.WriteAllLines(_identifier + "_full (" + logs.Count + ").txt", logs);
            File.WriteAllLines(_identifier + "_faillures ("+ faillures.Count + ").txt", faillures);
        }
    }
}
