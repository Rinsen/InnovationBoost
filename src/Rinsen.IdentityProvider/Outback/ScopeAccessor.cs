using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Scopes;

namespace Rinsen.IdentityProvider.Outback
{
    public class ScopeAccessor : IScopeAccessor
    {
        private readonly OutbackDbContext _outbackDbContext;

        public ScopeAccessor(OutbackDbContext outbackDbContext)
        {
            _outbackDbContext = outbackDbContext;
        }
        
        public Task<List<Scope>> GetScopes()
        {
            return _outbackDbContext.OutbackScopes.Include(m => m.ScopeClaims).Select(s => new Scope
            {
                ScopeName = s.ScopeName,
                ShowInDiscoveryDocument = s.ShowInDiscoveryDocument,
                Claims = s.ScopeClaims.Select(sc => new ScopeClaim { ClaimType = sc.Type }).ToList()
            }).ToListAsync();
        }
    }
}
