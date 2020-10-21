using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rinsen.OAuth.Controllers
{
    [ApiController]
    [Route(".well-known")]
    public class WellKnownController : ControllerBase    
    {

        public WellKnownController()
        {



        }

        [Route("openid-configuration")]
        public OpenIdConfiguration OpenIdConfiguration()
        {
            var host = $"https://{HttpContext.Request.Host}";

            return new OpenIdConfiguration
            {
                Issuer =host,
                JwksUri = $"{host}/.well-known/openid-configuration/jwks",
                AuthorizationEndpoint = $"{host}/connect/authorize",
                TokenEndpoint = $"{host}/connect/token",
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
            return new Root
            {
                Keys = new List<Key>
                {
                    new Key
                    {
                        Kty =  "RSA",
                        Use = "sig",
                        Kid = "5NFTZBIr-TBe6w4MiA158Q",
                        E = "AQAB",
                        N = "aaa",
                        Alg = "RS256"
                    }
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
        //public List<string> response_modes_supported { get; set; }

        [JsonPropertyName("token_endpoint_auth_methods_supported")]
        public List<string> TokenEndpointAuthMethodsSupported { get; set; }
        //public List<string> id_token_signing_alg_values_supported { get; set; }

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
        public List<Key> Keys { get; set; }
    }
}
