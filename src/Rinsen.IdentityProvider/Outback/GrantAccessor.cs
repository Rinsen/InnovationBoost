using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Grants;

namespace Rinsen.IdentityProvider.Outback
{
    public class GrantAccessor : IGrantAccessor
    {
        public Task<CodeGrant> GetCodeGrant(string code)
        {
            throw new NotImplementedException();
        }

        public Task<PersistedGrant> GetPersistedGrant(string clientId, string subjectId)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshTokenGrant> GetRefreshTokenGrant(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task SaveCodeGrant(CodeGrant codeGrant)
        {
            throw new NotImplementedException();
        }

        public Task SavePersistedGrant(PersistedGrant persistedGrant)
        {
            throw new NotImplementedException();
        }

        public Task SaveRefreshTokenGrant(RefreshTokenGrant refreshTokenGrant)
        {
            throw new NotImplementedException();
        }
    }
}
