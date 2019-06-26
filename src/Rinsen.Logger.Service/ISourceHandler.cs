using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public interface ISourceHandler
    {
        Task<Dictionary<string, int>> GetSourceIdsAsync(IEnumerable<string> sourceNames);
    }
}
