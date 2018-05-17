using System.Collections.Generic;
using System.Threading.Tasks;
using Rinsen.IdentityProvider.ExternalApplications;

namespace Rinsen.Logger.Service
{
    public class LogHandler
    {
        private readonly ILogWriter _logWriter;
        private readonly IExternalApplicationStorage _externalApplicationStorage;
        private readonly ILogEnvironmentHandler _logEnvironmentHandler;
        private readonly ISourceHandler _sourceHandler;

        public LogHandler(ILogWriter logWriter,
            IExternalApplicationStorage externalApplicationStorage,
            ILogEnvironmentHandler logEnvironmentHandler,
            ISourceHandler sourceHandler)
        {
            _logWriter = logWriter;
            _externalApplicationStorage = externalApplicationStorage;
            _logEnvironmentHandler = logEnvironmentHandler;
            _sourceHandler = sourceHandler;
        }

        public async Task<bool> CreateLogs(LogReport logReport)
        {
            var logApplication = await _externalApplicationStorage.GetFromApplicationKeyAsync(logReport.ApplicationKey);

            if (logApplication == default(ExternalApplication))
                return false;

            var logs = new List<Log>();
            foreach (var log in logReport.LogItems)
            {
                var environmentId = await _logEnvironmentHandler.GetLogEnvironmentIdAsync(log.EnvironmentName);
                var sourceId = await _sourceHandler.GetSourceIdAsync(log.SourceName);

                logs.Add(new Log
                {
                    ApplicationId = logApplication.Id,
                    LogLevel = log.LogLevel,
                    EnvironmentId = environmentId,
                    LogProperties = log.LogProperties,
                    MessageFormat = log.MessageFormat,
                    RequestId = log.RequestId,
                    SourceId = sourceId,
                    Timestamp = log.Timestamp
                });
            }

            await _logWriter.WriteLogsAsync(logs);

            return true;
        }
    }
}
