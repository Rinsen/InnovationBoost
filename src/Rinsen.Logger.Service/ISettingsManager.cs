using System;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public interface ISettingsManager
    {
        Task<T> GetValueOrDefault<T>(string key, Guid identityId);
        Task Set<T>(string key, Guid identityId, T setting);
    }
}
