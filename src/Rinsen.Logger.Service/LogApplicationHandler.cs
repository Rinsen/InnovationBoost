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

        public async Task<LogApplication> GetLogApplicationAsync(string applicationName)
        {
            var logApplication = await _logReader.GetLogApplicationAsync(applicationName);

            if (logApplication == default)
            {
                return await _logWriter.CreateLogApplicationAsync(applicationName, "");
            }

            return logApplication;
        }
    }
}
