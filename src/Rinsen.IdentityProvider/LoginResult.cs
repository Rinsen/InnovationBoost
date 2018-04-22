using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider
{
    public class LoginResult
    {
        public ClaimsPrincipal Principal { get; private set; }
        public bool Succeeded { get; private set; }
        public bool Failed { get { return !Succeeded; } }

        public static LoginResult Failure()
        {
            return new LoginResult() { Succeeded = false };
        }

        public static LoginResult Success(ClaimsPrincipal principal)
        {
            return new LoginResult() { Succeeded = true, Principal = principal };
        }
    }
}
