using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Rinsen.IdentityProvider.Token
{
    public class TokenOptions : RemoteAuthenticationOptions, IOptions<TokenOptions>
    {
        public string ApplicationKey { get; set; }
        public string ApplicationName { get; set; }
        public string IdentityServiceUrl { get { return _identityServiceUrl; } set { _identityServiceUrl = value.TrimEnd('/'); } }
        private string _identityServiceUrl;

        public TokenOptions Value { get { return this; } }

        public string AuthenticationScheme { get; set; } = "Cookies";
        public string ExternalUrlParamterName { get; set; } = "ExternalUrl";
        public string ApplicationKeyParameterName { get; set; } = "ApplicationKey";
        public string ApplicationNameParameterName { get; set; } = "ApplicationName";
        public string TokenParameterName { get; set; } = "AuthToken";
        public string HostParameterName { get; set; } = "Host";

    }
}
