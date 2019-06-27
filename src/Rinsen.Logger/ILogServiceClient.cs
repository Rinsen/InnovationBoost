using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public interface ILogServiceClient
    {
        Task ReportAsync(IEnumerable<LogItem> logItems);
    }
}
