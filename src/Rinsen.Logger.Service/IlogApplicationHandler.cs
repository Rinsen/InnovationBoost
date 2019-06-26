using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public interface ILogApplicationHandler
    {
        Task<LogApplication> GetLogApplicationAsync(string logApplicationName);
    }
}
