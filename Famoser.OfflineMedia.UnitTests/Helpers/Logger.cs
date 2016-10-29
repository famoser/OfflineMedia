using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Famoser.OfflineMedia.UnitTests.Helpers.Models;

namespace Famoser.OfflineMedia.UnitTests.Helpers
{
    /// <summary>
    /// Not completely thread safe implementation of a logger, but enough for unit tests
    /// </summary>
    public sealed class Logger : IDisposable
    {
        private readonly string _identifier;
        public Logger(string identifier)
        {
            _identifier = identifier;
        }

        private ConcurrentBag<LogEntry> _logs = new ConcurrentBag<LogEntry>();
        private ConcurrentBag<LogEntry> _safeLogs = new ConcurrentBag<LogEntry>();
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
            if (_logs.Count > 10000)
            {
                TemporarySafe();
            }
        }

        private bool isTemporarySaving = false;
        private async void TemporarySafe()
        {
            lock (this)
            {
                if (isTemporarySaving)
                    return;
                isTemporarySaving = true;
            }

            //wait for saving to finish
            while (_safeLogs != null)
            {
                await Task.Delay(100);
            }

            //switch collections
            _safeLogs = _logs;
            _hasEntryWithFaillure = _hasEntryWithFaillure || HasEntryWithFaillure();
            _logs = new ConcurrentBag<LogEntry>();

            //safe
            SafeLog();
        }

        private bool _hasEntryWithFaillure;
        public bool HasEntryWithFaillure()
        {
            return _hasEntryWithFaillure || _logs.Any(logEntry => logEntry.OutputFaillures().Any());
        }

        public string GetSavePath()
        {
            return Path.Combine(Path.GetTempPath(), "OfflineMediaTestResults", _identifier + "_*.txt");
        }

        private int _safeCounter = 0;

        private void SafeLog(bool useSafeCounter = true)
        {
            var logs = new List<string>();
            foreach (var logEntry in _safeLogs)
            {
                logs.AddRange(logEntry.OutputAll());
            }
            var faillures = new List<string>();
            foreach (var logEntry in _safeLogs)
            {
                faillures.AddRange(logEntry.OutputFaillures());
            }
            _safeLogs = null;

            var folder = Path.Combine(Path.GetTempPath(), "OfflineMediaTestResults");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var baseFileName = _identifier;
            if (!useSafeCounter || _safeCounter > 0)
            {
                baseFileName += 0;
            }

            File.WriteAllLines(Path.Combine(folder, baseFileName + "_full (" + logs.Count + ").txt"), logs);
            File.WriteAllLines(Path.Combine(folder, baseFileName + "_faillures (" + faillures.Count + ").txt"), faillures);
        }

        private bool _isDisposed;
        private void Dispose(bool dispose)
        {
            if (!_isDisposed)
            {
                if (dispose)
                {
                    _safeLogs = _logs;
                    _hasEntryWithFaillure = _hasEntryWithFaillure || HasEntryWithFaillure();
                    _logs = new ConcurrentBag<LogEntry>();
                    SafeLog(false);
                }
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
