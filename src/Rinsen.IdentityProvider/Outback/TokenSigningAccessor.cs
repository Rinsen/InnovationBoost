using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rinsen.Outback.Abstractons;

namespace Rinsen.IdentityProvider.Outback
{
    public class TokenSigningAccessor : ITokenSigningAccessor
    {
        public Task<SecurityKeyWithAlgorithm> GetSigningSecurityKey()
        {
            throw new NotImplementedException();
        }
    }
}
