using System;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Token
{
    public class TokenAuthenticationEvents : ITokenAuthenticationEvents
    {
        public Func<AuthenticationSucceededContext, Task> OnAuthenticationSuccess { get; set; } = context => Task.FromResult(0);

        public virtual Task AuthenticationSucceeded(AuthenticationSucceededContext context) => OnAuthenticationSuccess(context);
        
    }
}
