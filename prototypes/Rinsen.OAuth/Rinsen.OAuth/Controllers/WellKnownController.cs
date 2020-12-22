using Microsoft.AspNetCore.Mvc;
using Rinsen.Outback;
using Rinsen.Outback.Cryptography;
using Rinsen.Outback.WellKnown;
using System.Collections.Generic;

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
                CodeChallengeMethodsSupported = new List<string> { "S256" },
                FrontchannelLogoutSessionSupported = false,
                FrontchannelLogoutSupported = false,
                BackchannelLogoutSessionSupported = false,
                BackchannelLogoutSupported = false,
            };
        }

        [Route("openid-configuration/jwks")]
        public EllipticCurveJsonWebKeyModelKeys OpenIdConfigurationJwks()
        {
            var keyModel =  _ellipticCurveJsonWebKeyService.GetEllipticCurveJsonWebKeyModel();

            return new EllipticCurveJsonWebKeyModelKeys
            {
                Keys = new List<EllipticCurveJsonWebKeyModel>
                {
                    keyModel
                }
            };
        }
    }

    
}
