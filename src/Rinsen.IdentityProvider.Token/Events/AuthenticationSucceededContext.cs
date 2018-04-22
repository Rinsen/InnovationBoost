using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Token
{
    public class AuthenticationSucceededContext
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
    }
}
