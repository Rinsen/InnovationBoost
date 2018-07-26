using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class LogEnvironmentHandler : ILogEnvironmentHandler
    {
        private readonly ILogReader _logReader;
        private readonly ILogWriter _logWriter;
        private Dictionary<string, int> _logEnvironmentIds = new Dictionary<string, int>();

        public LogEnvironmentHandler(ILogReader logReader,
            ILogWriter logWriter)
        {
            _logReader = logReader;
            _logWriter = logWriter;
        }

        public async Task<int> GetLogEnvironmentIdAsync(string environmentName)
        {
            if (_logEnvironmentIds.ContainsKey(environmentName))
                return _logEnvironmentIds[environmentName];

            return await GetOrCreateLogEnvironmentIdAsync(environmentName);
        }

        private async Task<int> GetOrCreateLogEnvironmentIdAsync(string sourceName)
        {
            _logEnvironmentIds = await _logReader.GetLogEnvironmentIdsAsync();

            if (_logEnvironmentIds.ContainsKey(sourceName))
                return _logEnvironmentIds[sourceName];

            var logSource = await _logWriter.CreateLogEnvironmentAsync(sourceName);

            _logEnvironmentIds.Add(sourceName, logSource.Id);

            return logSource.Id;
        }
    }
}
