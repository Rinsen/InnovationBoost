using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Models;

namespace Rinsen.IdentityProvider.Outback
{
    public class WellKnownSigningAccessor : IWellKnownSigningAccessor
    {
        public Task<EllipticCurveJsonWebKeyModelKeys> GetEllipticCurveJsonWebKeyModelKeys()
        {
            throw new NotImplementedException();
        }
    }
}
