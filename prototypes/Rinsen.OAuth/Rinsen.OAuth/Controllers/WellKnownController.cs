using Microsoft.AspNetCore.Mvc;
using Rinsen.Outback;
using Rinsen.Outback.Cryptography;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rinsen.OAuth.Controllers
{
    [ApiController]
    [Route(".well-known")]
    public class WellKnownController : ControllerBase    
    {
        private readonly EllipticCurveJsonWebKeyService _ellipticCurveJsonWebKeyService;

        public WellKnownController(EllipticCurveJsonWebKeyService ellipticCurveJsonWebKeyService)
        {
            _ellipticCurveJsonWebKeyService = ellipticCurveJsonWebKeyService;
        }

        [Route("openid-configuration")]
        public OpenIdConfiguration OpenIdConfiguration()
        {
            var host = HttpContext.Request.Host.ToString();

            return new OpenIdConfiguration
            {
                Issuer = host,
                JwksUri = $"https://{host}/.well-known/openid-configuration/jwks",
                AuthorizationEndpoint = $"https://{host}/connect/authorize",
                TokenEndpoint = $"https://{host}/connect/token",
                TokenEndpointAuthMethodsSupported = new List<string> { "client_secret_basic" },
                GrantTypesSupported = new List<string> { "authorization_code", "client_credentials", "refresh_token" },
                CodeChallengeMethodsSupported = new List<string> { "plain", "S256" },
                FrontchannelLogoutSessionSupported = false,
                FrontchannelLogoutSupported = false,
                BackchannelLogoutSessionSupported = false,
                BackchannelLogoutSupported = false,
            };
        }

        [Route("openid-configuration/jwks")]
        public Root OpenIdConfigurationJwks()
        {
            var keyModel =  _ellipticCurveJsonWebKeyService.GetEllipticCurveJsonWebKeyModel();

            return new Root
            {
                Keys = new List<EllipticCurveJsonWebKeyModel>
                {
                    keyModel
                }
            };
        }
    }

    public class OpenIdConfiguration
    {
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("jwks_uri")]
        public string JwksUri { get; set; }

        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }

        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; }

        //public string userinfo_endpoint { get; set; }
        //public string end_session_endpoint { get; set; }
        //public string check_session_iframe { get; set; }
        //public string revocation_endpoint { get; set; }
        //public string introspection_endpoint { get; set; }
        //public string device_authorization_endpoint { get; set; }

        [JsonPropertyName("frontchannel_logout_supported")]
        public bool FrontchannelLogoutSupported { get; set; }

        [JsonPropertyName("frontchannel_logout_session_supported")]
        public bool FrontchannelLogoutSessionSupported { get; set; }

        [JsonPropertyName("backchannel_logout_supported")]
        public bool BackchannelLogoutSupported { get; set; }

        [JsonPropertyName("backchannel_logout_session_supported")]
        public bool BackchannelLogoutSessionSupported { get; set; }

        [JsonPropertyName("scopes_supported")]
        public List<string> ScopesSupported { get; set; }

        [JsonPropertyName("claims_supported")]
        public List<object> ClaimsSupported { get; set; }

        [JsonPropertyName("grant_types_supported")]
        public List<string> GrantTypesSupported { get; set; }

        [JsonPropertyName("response_types_supported")]
        public List<string> ResponseTypesSupported { get; set; }

        [JsonPropertyName("token_endpoint_auth_methods_supported")]
        public List<string> TokenEndpointAuthMethodsSupported { get; set; }

        [JsonPropertyName("subject_types_supported")]
        public List<string> SubjectTypesSupported { get; set; }

        [JsonPropertyName("code_challenge_methods_supported")]
        public List<string> CodeChallengeMethodsSupported { get; set; }

        [JsonPropertyName("request_parameter_supported")]
        public bool RequestParameterSupported { get; set; }
    }

    public class Key
    {
        [JsonPropertyName("kty")]
        public string Kty { get; set; }

        [JsonPropertyName("use")]
        public string Use { get; set; }

        [JsonPropertyName("kid")]
        public string Kid { get; set; }

        [JsonPropertyName("e")]
        public string E { get; set; }

        [JsonPropertyName("n")]
        public string N { get; set; }

        [JsonPropertyName("alg")]
        public string Alg { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("keys")]
        public List<EllipticCurveJsonWebKeyModel> Keys { get; set; }
    }
}
