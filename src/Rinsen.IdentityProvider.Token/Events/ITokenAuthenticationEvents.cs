using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Token
{
    public interface ITokenAuthenticationEvents
    {
        Task AuthenticationSucceeded(AuthenticationSucceededContext context);
    }
}
