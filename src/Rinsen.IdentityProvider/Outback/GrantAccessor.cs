using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Grants;

namespace Rinsen.IdentityProvider.Outback
{

    public class GrantAccessor : IGrantAccessor
    {
        private readonly OutbackDbContext _outbackDbContext;

        public GrantAccessor(OutbackDbContext outbackDbContext)
        {
            _outbackDbContext = outbackDbContext;
        }

        public async Task<CodeGrant> GetCodeGrant(string code)
        {
            var outbackCodeGrant = await _outbackDbContext.CodeGrants.Include(m => m.Client).SingleOrDefaultAsync(m => m.Code == code);

            if (outbackCodeGrant == default)
            {
                throw new Exception($"Code grant not found for code {code}");
            }

            return new CodeGrant
            {
                ClientId = outbackCodeGrant.Client.ClientId,
                Code = outbackCodeGrant.Code,
                CodeChallange = outbackCodeGrant.CodeChallange,
                CodeChallangeMethod = outbackCodeGrant.CodeChallangeMethod,
                Created = outbackCodeGrant.Created,
                Expires = outbackCodeGrant.Expires,
                Nonce = outbackCodeGrant.Nonce,
                RedirectUri = outbackCodeGrant.RedirectUri,
                Resolved = outbackCodeGrant.Resolved,
                Scope = outbackCodeGrant.Scope,
                State = outbackCodeGrant.State,
                SubjectId = outbackCodeGrant.SubjectId.ToString(),
            };
        }

        public async Task<PersistedGrant> GetPersistedGrant(string clientId, Guid subjectId)
        {
            var outbackPersistedGrant = await _outbackDbContext.PersistedGrants.Include(m => m.Client).SingleOrDefaultAsync(m => m.Client.ClientId == clientId && m.SubjectId == subjectId);

            if (outbackPersistedGrant == default)
            {
                throw new Exception($"Persisted grant not found for client {clientId} and subject {subjectId}");
            }

            return new PersistedGrant
            {
                ClientId = outbackPersistedGrant.Client.ClientId,
                Created = outbackPersistedGrant.Created,
                Expires = outbackPersistedGrant.Expires,
                Scope = outbackPersistedGrant.Scope,
                SubjectId = outbackPersistedGrant.SubjectId.ToString()
            };
        }

        public async Task<RefreshTokenGrant> GetRefreshTokenGrant(string refreshToken)
        {
            var outbackPersistedGrant = await _outbackDbContext.RefreshTokenGrants.Include(m => m.Client).SingleOrDefaultAsync(m => m.RefreshToken == refreshToken);

            if (outbackPersistedGrant == default)
            {
                throw new Exception($"Refresh token not found {refreshToken}");
            }

            return new RefreshTokenGrant
            {
                ClientId = outbackPersistedGrant.Client.ClientId,
                Created = outbackPersistedGrant.Created,
                Expires = outbackPersistedGrant.Expires,
                Scope = outbackPersistedGrant.Scope,
                SubjectId = outbackPersistedGrant.SubjectId.ToString(),
                RefreshToken = outbackPersistedGrant.RefreshToken,
                Resolved = outbackPersistedGrant.Resolved
            };
        }

        public async Task SaveCodeGrant(CodeGrant codeGrant)
        {
            var clientIntId = await _outbackDbContext.Clients.Where(m => m.ClientId == codeGrant.ClientId).Select(m => m.Id).SingleAsync();

            var outbackCodeGrant = new OutbackCodeGrant
            {
                ClientId = clientIntId,
                Code = codeGrant.Code,
                CodeChallange = codeGrant.CodeChallange,
                CodeChallangeMethod = codeGrant.CodeChallangeMethod,
                Created = codeGrant.Created,
                Expires = codeGrant.Expires,
                Nonce = codeGrant.Nonce,
                RedirectUri = codeGrant.RedirectUri,
                Resolved = codeGrant.Resolved,
                Scope = codeGrant.Scope,
                State = codeGrant.State,
                SubjectId = Guid.Parse(codeGrant.SubjectId),
            };

            await _outbackDbContext.CodeGrants.AddAsync(outbackCodeGrant);

            await _outbackDbContext.SaveChangesAsync();
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
