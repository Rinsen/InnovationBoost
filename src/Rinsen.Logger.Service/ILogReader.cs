using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Rinsen.Logger.Service
{
    public interface ILogReader
    {
        Task<List<LogEnvironment>> GetLogEnvironmentsAsync();
        Task<Dictionary<string, int>> GetLogEnvironmentIdsAsync();
        Task<List<LogSource>> GetLogSourcesAsync();
        Task<Dictionary<string, int>> GetLogSourceIdsAsync();
        Task<IEnumerable<LogView>> GetLogsAsync(DateTimeOffset from, DateTimeOffset to, IEnumerable<int> logApplications, IEnumerable<int> logEnvironments, IEnumerable<int> logSources, IEnumerable<int> logLevels, int take = 10000);
    }
}
