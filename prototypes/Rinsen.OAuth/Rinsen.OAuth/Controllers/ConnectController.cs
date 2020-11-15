using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Rinsen.Outback;
using Rinsen.Outback.Clients;
using Rinsen.Outback.Cryptography;
using Rinsen.Outback.Grant;
using Rinsen.Outback.Models;
using Rinsen.Outback.WellKnown;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.OAuth.Controllers
{
    [Route("connect")]
    public class ConnectController : Controller
    {
        private readonly EllipticCurveJsonWebKeyService _ellipticCurveJsonWebKeyService;
        private readonly GrantService _grantService;
        private readonly ClientService _clientService;
        private readonly ClientValidator _clientValidator;

        public ConnectController(EllipticCurveJsonWebKeyService ellipticCurveJsonWebKeyService,
            GrantService grantService,
            ClientService clientService,
            ClientValidator clientValidator)
        {
            _ellipticCurveJsonWebKeyService = ellipticCurveJsonWebKeyService;
            _grantService = grantService;
            _clientService = clientService;
            _clientValidator = clientValidator;
        }

        [HttpGet]
        [Route("authorize")]
        public async Task<IActionResult> Authorize([FromQuery]AuthorizeModel model)
        {
            if (ModelState.IsValid)
            {
                // Get client
                var client = await _clientService.GetClient(model.ClientId);

                // Validare scopes and return url
                if(!_clientValidator.IsScopeValid(client, model.Scope))
                {
                    throw new SecurityException();
                }

                // Validare scopes and return url
                if (!_clientValidator.IsRedirectUriValid(client, model.RedirectUri))
                {
                    throw new SecurityException();
                }

                // Collect consent if needed
                if (client.ConsentRequired)
                {
                    return View("Consent");
                }

                // Generate and store grant
                var code = _grantService.CreateAndStoreGrant(client.ClientId, "user123", model.CodeChallenge, model.CodeChallengeMethod, model.Nonce, model.RedirectUri, model.Scope, model.State, model.ResponseType);

                // Return code 
                return View(new AuthorizeResponse
                {
                    Code = code,
                    FormPostUri = model.RedirectUri,
                    Scope = model.Scope,
                    SessionState = null,
                    State = model.State
                });
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("token")]
        public IActionResult Token([FromForm] TokenModel model)
        {
            if (ModelState.IsValid)
            {
                // Get client

                // Validate client secret if needed

                var persistedGrant = _grantService.GetGrant(model.Code);

                // Validate code
                using var sha256 = SHA256.Create();
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.CodeVerifier));
                var codeChallenge = WebEncoders.Base64UrlEncode(challengeBytes);

                if (codeChallenge != persistedGrant.CodeChallange)
                {
                    throw new SecurityException("Code cerifier is not matching code challenge");
                }

                // Validate return url if provided

                var tokenHandler = new JwtSecurityTokenHandler();
                var identityTokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim("sub", persistedGrant.SubjectId),
                        }),
                    TokenType = null,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Issuer = HttpContext.Request.Host.ToString(),
                    IssuedAt = DateTime.UtcNow,
                    Audience = persistedGrant.ClientId,
                    SigningCredentials = new SigningCredentials(_ellipticCurveJsonWebKeyService.GetECDsaSecurityKey(), SecurityAlgorithms.EcdsaSha256),
                };

                identityTokenDescriptor.Claims = new Dictionary<string, object> { { "nonce", persistedGrant.Nonce } };
                
                var identityToken = tokenHandler.CreateToken(identityTokenDescriptor);
                var identityTokenString = tokenHandler.WriteToken(identityToken);

                var accessTokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim("sub", persistedGrant.SubjectId),
                        }),
                    TokenType = "at+jwt",
                    Expires = DateTime.UtcNow.AddDays(7),
                    Issuer = HttpContext.Request.Host.ToString(),
                    IssuedAt = DateTime.UtcNow,
                    Audience = persistedGrant.ClientId,
                    SigningCredentials = new SigningCredentials(_ellipticCurveJsonWebKeyService.GetECDsaSecurityKey(), SecurityAlgorithms.EcdsaSha256),
                };

                accessTokenDescriptor.Claims = new Dictionary<string, object> { { "client_id", persistedGrant.ClientId } };
                accessTokenDescriptor.Claims.Add("scope", persistedGrant.Scope);

                var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
                var tokenString = tokenHandler.WriteToken(accessToken);

                Response.Headers.Add("Cache-Control", "no-store");
                Response.Headers.Add("Pragma", "no-cache");

                return Json(new TokenResponse
                {
                    AccessToken = tokenString,
                    IdentityToken = identityTokenString,
                    ExpiresIn = 3600, 
                    TokenType = "Bearer"
                });
            }

            return BadRequest(ModelState);
        }
        // EndSession

        // userinfo

        // checksession

        // revocation

        // introspect

        // deviceauthorization

    }

    
}
