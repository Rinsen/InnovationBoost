using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class SourceHandler : ISourceHandler
    {
        private readonly ILogReader _logReader;
        private readonly ILogWriter _logWriter;

        public SourceHandler(ILogReader logReader,
            ILogWriter logWriter)
        {
            _logReader = logReader;
            _logWriter = logWriter;
        }

        public async Task<Dictionary<string, int>> GetSourceIdsAsync(IEnumerable<string> sourceNames)
        {
            var logSources = await _logReader.GetLogSourcesAsync();

            var logSourcesDictionary = new Dictionary<string, int>();

            foreach (var logSource in logSources)
            {
                logSourcesDictionary.Add(logSource.Name, logSource.Id);
            }

            foreach (var name in sourceNames)
            {
                if (!logSourcesDictionary.ContainsKey(name))
                {
                    var logEnvironment = await _logWriter.CreateLogSourceAsync(name);

                    logSourcesDictionary.Add(logEnvironment.Name, logEnvironment.Id);
                }
            }

            return logSourcesDictionary;
        }
    }
}
