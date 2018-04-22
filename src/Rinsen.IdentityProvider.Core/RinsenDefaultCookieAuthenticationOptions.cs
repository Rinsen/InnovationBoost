using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Rinsen.IdentityProvider.Core
{
    public class RinsenDefaultCookieAuthenticationOptions : CookieAuthenticationOptions
    {
        public RinsenDefaultCookieAuthenticationOptions(string connectionString)
        {
            SessionStore = new SqlTicketStore(new SessionStorage(connectionString));
            Cookie.SecurePolicy = CookieSecurePolicy.Always;
        }
    }
}
