using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class LogEnvironmentHandler : ILogEnvironmentHandler
    {
        private readonly ILogReader _logReader;
        private readonly ILogWriter _logWriter;

        public LogEnvironmentHandler(ILogReader logReader,
            ILogWriter logWriter)
        {
            _logReader = logReader;
            _logWriter = logWriter;
        }

        public async Task<Dictionary<string, int>> GetLogEnvironmentIdsAsync(IEnumerable<string> environmentNames)
        {
            var logEnvironments = await _logReader.GetLogEnvironmentsAsync();

            var logEnvironmentsDictionary = new Dictionary<string, int>();

            foreach (var logEnvironment in logEnvironments)
            {
                logEnvironmentsDictionary.Add(logEnvironment.Name, logEnvironment.Id);
            }

            foreach (var name in environmentNames)
            {
                if (!logEnvironmentsDictionary.ContainsKey(name))
                {
                    var logEnvironment = await _logWriter.CreateLogEnvironmentAsync(name);

                    logEnvironmentsDictionary.Add(logEnvironment.Name, logEnvironment.Id);
                }
            }

            return logEnvironmentsDictionary;
        }
    }
}
