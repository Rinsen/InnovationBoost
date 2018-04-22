using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Core
{
    public interface ISessionStorage
    {
        Task CreateAsync(Session session);
        Task DeleteAsync(string sessionId);
        Task<IEnumerable<Session>> GetAsync(Guid identityId);
        Task<Session> GetAsync(string sessionId);
        Task UpdateAsync(Session session);
    }
}
