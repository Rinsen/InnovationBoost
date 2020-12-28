using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Grants;

namespace Rinsen.OAuth.InMemoryStorage
{
    public class GrantStorage : IGrantStorage
    {
        private readonly ConcurrentDictionary<string, CodeGrant> _persistedGrants = new ConcurrentDictionary<string, CodeGrant>();

        public Task<CodeGrant> GetCodeGrant(string code)
        {
            return Task.FromResult(_persistedGrants.GetValueOrDefault(code));
        }

        public Task<PersistedGrant> GetPersistedGrant(string code)
        {
            throw new System.NotImplementedException();
        }

        public Task<RefreshTokenGrant> GetRefreshTokenGrant(string code)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveCodeGrant(CodeGrant grant)
        {
            _persistedGrants.TryAdd(grant.Code, grant);

            return Task.CompletedTask;
        }

        public Task SavePersistedGrant(PersistedGrant persistedGrant)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveRefreshTokenGrant(RefreshTokenGrant refreshTokenGrant)
        {
            throw new System.NotImplementedException();
        }
    }
}
