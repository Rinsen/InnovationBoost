using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rinsen.Logger
{
    public class LogQueue : ILogQueue
    {
        readonly ConcurrentQueue<LogItem> _logs;
        readonly LogOptions _logOptions;

        public LogQueue(LogOptions logOptions)
        {
            _logs = new ConcurrentQueue<LogItem>();
            _logOptions = logOptions;
        }

        public void AddLog(string sourceName, string requestId, LogLevel logLevel, string messageFormat, IEnumerable<LogProperty> logProperties)
        {
            if (_logs.Count > _logOptions.QueueMazSize)
                return;

            _logs.Enqueue(new LogItem { SourceName = sourceName, EnvironmentName = _logOptions.EnvironmentName, RequestId = requestId, LogLevel = logLevel, MessageFormat = messageFormat, LogProperties = logProperties, Timestamp = DateTimeOffset.Now });
        }

        public void AddLogs(IEnumerable<LogItem> logs)
        {
            foreach (var logItem in logs)
            {
                _logs.Enqueue(logItem);
            }
        }

        public void GetReportedLogs(List<LogItem> logItems)
        {
            logItems.Clear();

            if (_logs.IsEmpty)
            {
                return;
            }

            int logCount = _logs.Count;
            var resultSize = logCount < logItems.Capacity ? logCount : logItems.Capacity;

            for (int i = 0; i < resultSize; i++)
            {
                if (!_logs.TryDequeue(out LogItem logItem))
                    break;

                logItems.Add(logItem);
            }
        }
    }
}
