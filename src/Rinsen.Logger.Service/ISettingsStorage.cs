using System;
using System.Threading.Tasks;

namespace Rinsen.Logger.Service
{
    public interface ISettingsStorage
    {
        Task Create(Setting setting);
        Task<Setting> Get(string key, Guid identityId);
        Task Update(Setting setting);
    }
}