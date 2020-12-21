using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Rinsen.Outback;
using Rinsen.Outback.Clients;
using Rinsen.Outback.Grants;
using Rinsen.Outback.Models;
using System;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rinsen.OAuth.Controllers
{
    [Route("connect")]
    public class ConnectController : Controller
    {
        private readonly GrantService _grantService;
        private readonly ClientService _clientService;
        private readonly TokenFactory _tokenFactory;

        public ConnectController(
            GrantService grantService,
            ClientService clientService,
            TokenFactory tokenFactory)
        {
            _grantService = grantService;
            _clientService = clientService;
            _tokenFactory = tokenFactory;
        }

        [HttpGet]
        [Route("authorize")]
        public async Task<IActionResult> Authorize([FromQuery]AuthorizeModel model)
        {
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                            new Claim("sub", "Kalle123")
                }));

            if (ModelState.IsValid)
            {
                var client = await _clientService.GetClient(model);

                string code;
                if (client.ConsentRequired)
                {
                    code = await _grantService.GetCodeForExistingConsent(client, User, model);

                    if (string.IsNullOrEmpty(code))
                    {
                        return View("Consent");
                    }
                }
                else
                {
                    // Generate and store grant
                    code = await _grantService.CreateAndStoreGrant(client, User, model);
                }

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
        public async Task<IActionResult> Token([FromForm] TokenModel model)
        {
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                            new Claim("sub", "Kalle123")
                }));

            if (ModelState.IsValid)
            {
                var client = await GetClient(model);

                if (!ClientValidator.IsGrantTypeSupported(client, model.GrantType))
                {
                    // Client does not support grant type
                    throw new SecurityException();
                }

                return model.GrantType switch
                {
                    "client_credentials" => await GetTokenForClientCredentials(model, client),
                    "authorization_code" => await GetTokenForAuthorizationCode(model, client),
                    "refresh_token" => await GetTokenForRefreshToken(model, client),
                    _ => throw new SecurityException($"Grant {model.GrantType} is not supported"),
                };
            }

            return BadRequest(ModelState);
        }

        private async Task<Client> GetClient(TokenModel model)
        {
            // Basic auth or post paramaters for client auth and not both
            if (Request.Headers.ContainsKey("Authorization") && (!string.IsNullOrEmpty(model.ClientId) || !string.IsNullOrEmpty(model.ClientSecret)))
            {
                // Only one type of credentials is supported at the same time
                throw new SecurityException();
            }

            if (Request.Headers.ContainsKey("Authorization"))
            {
                var value = Request.Headers["Authorization"];

                if (value == StringValues.Empty)
                {
                    // Empty Authorization header is not supported
                    throw new SecurityException();
                }

                var authHeaderValue = Base64UrlEncoder.Decode(value);
                var parts = authHeaderValue.Split(':', 2, StringSplitOptions.TrimEntries);

                if (!parts[0].StartsWith("Basic "))
                {
                    throw new SecurityException();
                }
                else
                {
                    return await _clientService.GetClient(parts[1][6..], parts[1]);
                }
            }
            else if (!string.IsNullOrEmpty(model.ClientId) && !string.IsNullOrEmpty(model.ClientSecret))
            {
                // Get client
                return await _clientService.GetClient(model.ClientId, model.ClientSecret);
            }
            else
            {
                throw new SecurityException(); // No credentials
            }
        }

        private Task<IActionResult> GetTokenForClientCredentials(TokenModel model, Client client)
        {
            //_tokenFactory.CreateTokenResponse(client)

            throw new NotImplementedException();
        }

        private async Task<IActionResult> GetTokenForAuthorizationCode(TokenModel model, Client client)
        {
            var persistedGrant = await _grantService.GetGrant(model.Code, client.ClientId, model.CodeVerifier);

            // Validate return url if provided
            if (!string.Equals(persistedGrant.RedirectUri, model.RedirectUri))
            {
                throw new SecurityException();
            }

            var tokenResponse = _tokenFactory.CreateTokenResponse(User, client, persistedGrant, HttpContext.Request.Host.ToString());
            
            AddCacheControlHeader();

            return Json(tokenResponse);
        }

        private async Task<IActionResult> GetTokenForRefreshToken(TokenModel model, Client client)
        {
            var persistedGrant = await _grantService.GetGrant(model.RefreshToken, client.ClientId);

            // Validate return url if provided
            if (!ClientValidator.IsRedirectUriValid(client, model.RedirectUri))
            {
                throw new SecurityException();
            }

            var tokenResponse = _tokenFactory.CreateTokenResponse(User, client, persistedGrant, HttpContext.Request.Host.ToString());

            AddCacheControlHeader();

            return Json(tokenResponse);
        }

        private void AddCacheControlHeader()
        {
            Response.Headers.Add("Cache-Control", "no-store");
            Response.Headers.Add("Pragma", "no-cache");
        }

        // EndSession

        // userinfo

        // checksession

        // revocation

        // introspect

        // deviceauthorization

    }

    
}
