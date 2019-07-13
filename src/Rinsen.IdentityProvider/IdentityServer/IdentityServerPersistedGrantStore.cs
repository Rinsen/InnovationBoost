using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServerPersistedGrantStore : IPersistedGrantStore
    {
        private readonly IdentityServerDbContext _identityServerDbContext;

        public IdentityServerPersistedGrantStore(IdentityServerDbContext identityServerDbContext)
        {
            _identityServerDbContext = identityServerDbContext;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var identityServerPersistedGrants =  await _identityServerDbContext.IdentityServerPersistedGrants.Where(m => m.SubjectId == subjectId).ToListAsync();

            return identityServerPersistedGrants.Select(pg => new PersistedGrant {
                    ClientId = pg.ClientId,
                    CreationTime = pg.CreationTime.DateTime,
                    Data = pg.Data,
                    Expiration = pg.Expiration.HasValue ? pg.Expiration.Value.DateTime : (DateTime?)null,
                    Key = pg.Key,
                    SubjectId = pg.SubjectId,
                    Type = pg.Type
                }).ToList();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var identityServerPersistedGrant = await _identityServerDbContext.IdentityServerPersistedGrants.FirstOrDefaultAsync(m => m.Key == key);

            if (identityServerPersistedGrant == default)
            {
                return null;
            }

            return new PersistedGrant
            {
                ClientId = identityServerPersistedGrant.ClientId,
                CreationTime = identityServerPersistedGrant.CreationTime.DateTime,
                Data = identityServerPersistedGrant.Data,
                Expiration = identityServerPersistedGrant.Expiration.HasValue ? identityServerPersistedGrant.Expiration.Value.DateTime : (DateTime?)null,
                Key = identityServerPersistedGrant.Key,
                SubjectId = identityServerPersistedGrant.SubjectId,
                Type = identityServerPersistedGrant.Type
            };
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var identityServerPersistedGrants = await _identityServerDbContext.IdentityServerPersistedGrants.Where(m => m.SubjectId == subjectId && m.ClientId == clientId).ToListAsync();

            _identityServerDbContext.IdentityServerPersistedGrants.RemoveRange(identityServerPersistedGrants);

            await _identityServerDbContext.SaveChangesAsync();
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var identityServerPersistedGrants = await _identityServerDbContext.IdentityServerPersistedGrants.Where(m => m.SubjectId == subjectId && m.ClientId == clientId).ToListAsync();

            _identityServerDbContext.IdentityServerPersistedGrants.RemoveRange(identityServerPersistedGrants);

            await _identityServerDbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(string key)
        {
            var identityServerPersistedGrants = await _identityServerDbContext.IdentityServerPersistedGrants.Where(m => m.Key == key).ToListAsync();

            _identityServerDbContext.IdentityServerPersistedGrants.RemoveRange(identityServerPersistedGrants);

            await _identityServerDbContext.SaveChangesAsync();
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            var pg =  new IdentityServerPersistedGrant
            {
                ClientId = grant.ClientId,
                CreationTime = grant.CreationTime,
                Data = grant.Data,
                Expiration = grant.Expiration,
                Key = grant.Key,
                SubjectId = grant.SubjectId,
                Type = grant.Type
            };

            await _identityServerDbContext.IdentityServerPersistedGrants.AddAsync(pg);

            await _identityServerDbContext.SaveChangesAsync();
        }

    }
}
