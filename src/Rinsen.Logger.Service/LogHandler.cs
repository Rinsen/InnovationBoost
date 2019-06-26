using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class LogHandler
    {
        private readonly ILogWriter _logWriter;
        private readonly ILogEnvironmentHandler _logEnvironmentHandler;
        private readonly ISourceHandler _sourceHandler;

        public LogHandler(ILogWriter logWriter,
            ILogEnvironmentHandler logEnvironmentHandler,
            ISourceHandler sourceHandler)
        {
            _logWriter = logWriter;
            _logEnvironmentHandler = logEnvironmentHandler;
            _sourceHandler = sourceHandler;
        }

        public async Task CreateLogs(List<LogItem> logItems, int logApplicationId)
        {
            var logEnvironments = logItems.Select(m => m.EnvironmentName).Distinct().ToArray();
            var logSourceNames = logItems.Select(m => m.SourceName).Distinct().ToArray();

            var logEnviropnments = await _logEnvironmentHandler.GetLogEnvironmentIdsAsync(logEnvironments);
            var logSources = await _sourceHandler.GetSourceIdsAsync(logSourceNames);

            var logs = new List<Log>(logItems.Count);
            foreach (var log in logItems)
            {
                logs.Add(new Log
                {
                    ApplicationId = logApplicationId,
                    LogLevel = log.LogLevel,
                    EnvironmentId = logEnviropnments[log.EnvironmentName],
                    LogProperties = log.LogProperties,
                    MessageFormat = log.MessageFormat,
                    RequestId = log.RequestId,
                    SourceId = logSources[log.SourceName],
                    Timestamp = log.Timestamp
                });
            }

            await _logWriter.WriteLogsAsync(logs);
        }
    }
}

