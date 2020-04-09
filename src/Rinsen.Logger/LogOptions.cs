using System;
using Microsoft.Extensions.Logging;

namespace Rinsen.Logger
{
    public class LogOptions
    {
        public LogOptions()
        {
            MaxQueueSize = 2000;
            MaxBatchSize = 200;
            TimeToSleepBetweenBatches = new TimeSpan(0, 0, 5);
            MinLevel = LogLevel.Information;
        }

        public int MaxQueueSize { get; set; }

        public int MaxBatchSize { get; set; }

        public TimeSpan TimeToSleepBetweenBatches { get; set; }

        public string EnvironmentName { get; set; }

        public string ApplicationLogKey { get; set; }

        public string LogServiceUri { get; set; }

        public LogLevel MinLevel { get; set; }
    }
}
