using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Rinsen.Logger
{
    public interface ILogQueue
    {
        void AddLog(string sourceName, string requestId, LogLevel logLevel, string messageFormat, IEnumerable<LogProperty> logProperties);
        void GetReportedLogs(List<LogItem> logItems);
        void AddLogs(IEnumerable<LogItem> logs);
    }
}
