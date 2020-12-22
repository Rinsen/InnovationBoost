using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Grants;

namespace Rinsen.OAuth.InMemoryStorage
{
    public class GrantStorage : IGrantStorage
    {
        private readonly ConcurrentDictionary<string, Grant> _persistedGrants = new ConcurrentDictionary<string, Grant>();

        public Task<Grant> GetGrant(string code)
        {
            return Task.FromResult(_persistedGrants.GetValueOrDefault(code));
        }

        public Task Save(Grant grant)
        {
            _persistedGrants.TryAdd(grant.Code, grant);

            return Task.CompletedTask;
        }
    }
}
