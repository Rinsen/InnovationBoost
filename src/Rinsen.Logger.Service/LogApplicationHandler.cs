using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class LogApplicationHandler : ILogApplicationHandler
    {
        private readonly ILogReader _logReader;
        private readonly ILogWriter _logWriter;

        public LogApplicationHandler(ILogReader logReader,
            ILogWriter logWriter)
        {
            _logReader = logReader;
            _logWriter = logWriter;
        }

        public async Task<LogApplication> GetLogApplicationAsync(string applicationId, string displayName)
        {
            var logApplication = await _logReader.GetLogApplicationAsync(applicationId);

            if (logApplication == default)
            {
                return await _logWriter.CreateLogApplicationAsync(applicationId, displayName);
            }

            if (logApplication.DisplayName != displayName)
            {
                logApplication.DisplayName = displayName;

                await _logWriter.UpdateLogApplicationAsync(logApplication);
            }

            return logApplication;
        }
    }
}
