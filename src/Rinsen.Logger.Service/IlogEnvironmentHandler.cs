using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public interface ILogEnvironmentHandler
    {
        Task<int> GetLogEnvironmentIdAsync(string environmentName);
    }
}
