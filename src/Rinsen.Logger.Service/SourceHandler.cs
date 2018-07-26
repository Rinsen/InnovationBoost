using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public class SourceHandler : ISourceHandler
    {
        private readonly ILogReader _logReader;
        private readonly ILogWriter _logWriter;
        private Dictionary<string, int> _logSourceIds = new Dictionary<string, int>();

        public SourceHandler(ILogReader logReader,
            ILogWriter logWriter)
        {
            _logReader = logReader;
            _logWriter = logWriter;
        }

        public async Task<int> GetSourceIdAsync(string sourceName)
        {
            if (_logSourceIds.ContainsKey(sourceName))
                return _logSourceIds[sourceName];

            return await GetOrCreateSourceIdAsync(sourceName);
        }

        private async Task<int> GetOrCreateSourceIdAsync(string sourceName)
        {
            _logSourceIds = await _logReader.GetLogSourceIdsAsync();
            
            if (_logSourceIds.ContainsKey(sourceName))
                return _logSourceIds[sourceName];

            var logSource = await _logWriter.CreateLogSourceAsync(sourceName);

            _logSourceIds.Add(sourceName, logSource.Id);

            return logSource.Id;
        }
    }
}
