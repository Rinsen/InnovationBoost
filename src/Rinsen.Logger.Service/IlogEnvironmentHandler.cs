using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public interface ILogEnvironmentHandler
    {
        Task<Dictionary<string, int>> GetLogEnvironmentIdsAsync(IEnumerable<string> environmentName);
    }
}
