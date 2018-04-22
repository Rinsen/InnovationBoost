using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Rinsen.Logger
{
    public class LogItem
    {
        public int Id { get; set; }

        public string SourceName { get; set; }

        public string EnvironmentName { get; set; }

        public string RequestId { get; set; }

        public LogLevel LogLevel { get; set; }

        public string MessageFormat { get; set; }

        public IEnumerable<LogProperty> LogProperties { get; set; }

        public DateTimeOffset Timestamp { get; set; }

    }
}
