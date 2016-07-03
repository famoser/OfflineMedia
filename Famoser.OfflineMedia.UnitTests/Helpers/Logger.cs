using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentBag<LogEntry> _logs = new ConcurrentBag<LogEntry>();
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

        public string GetSavePath()
        {
            return Path.Combine(Path.GetTempPath(), "OfflineMediaTestResults", _identifier + "_*.txt");
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

            var folder = Path.Combine(Path.GetTempPath(), "OfflineMediaTestResults");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            File.WriteAllLines(Path.Combine(folder, _identifier + "_full (" + logs.Count + ").txt"), logs);
            File.WriteAllLines(Path.Combine(folder, _identifier + "_faillures (" + faillures.Count + ").txt"), faillures);
        }
    }
}
