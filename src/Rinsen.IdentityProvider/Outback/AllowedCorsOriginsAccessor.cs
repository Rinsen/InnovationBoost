using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rinsen.Outback.Abstractons;

namespace Rinsen.IdentityProvider.Outback
{
    public class AllowedCorsOriginsAccessor : IAllowedCorsOriginsAccessor
    {
        public Task<HashSet<string>> GetOrigins()
        {
            throw new NotImplementedException();
        }
    }
}
