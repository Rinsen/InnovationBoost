using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.Outback.Abstractons;

namespace Rinsen.IdentityProvider.Outback
{
    public class WellKnownScopeAccessor : IWellKnownScopeAccessor
    {
        private readonly OutbackDbContext _outbackDbContext;

        public WellKnownScopeAccessor(OutbackDbContext outbackDbContext)
        {
            _outbackDbContext = outbackDbContext;
        }

        public Task<List<string>> GetScopes()
        {
            return _outbackDbContext.OutbackScopes.Where(m => m.ShowInDiscoveryDocument == true).Select(m => m.ScopeName).ToListAsync();
        }
    }
}
