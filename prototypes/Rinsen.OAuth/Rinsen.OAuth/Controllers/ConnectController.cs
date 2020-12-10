using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Rinsen.Outback;
using Rinsen.Outback.Clients;
using Rinsen.Outback.Grant;
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
        private readonly ClientValidator _clientValidator;
        private readonly TokenFactory _tokenFactory;

        public ConnectController(
            GrantService grantService,
            ClientService clientService,
            ClientValidator clientValidator,
            TokenFactory tokenFactory)
        {
            _grantService = grantService;
            _clientService = clientService;
            _clientValidator = clientValidator;
            _tokenFactory = tokenFactory;
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                            new Claim("sub", "Kalle123")
                }));
        }

        [HttpGet]
        [Route("authorize")]
        public async Task<IActionResult> Authorize([FromQuery]AuthorizeModel model)
        {
            if (ModelState.IsValid)
            {
                var client = await _clientService.GetClient(model.ClientId);

                if(!_clientValidator.IsScopeValid(client, model.Scope))
                {
                    throw new SecurityException();
                }

                if (!_clientValidator.IsRedirectUriValid(client, model.RedirectUri))
                {
                    throw new SecurityException();
                }

                if (client.ConsentRequired)
                {
                    return View("Consent");
                }

                // Generate and store grant
                var code = await _grantService.CreateAndStoreGrant(client, User, model);

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
            if (ModelState.IsValid)
            {
                var client = await GetClient(model);

                if (!_clientValidator.IsGrantTypeSupported(client, model.GrantType))
                {
                    // Client does not support grant type
                    throw new SecurityException();
                }

                return model.GrantType switch
                {
                    "client_credentials" => throw new NotImplementedException(),
                    "authorization_code" => await GetTokenForAuthorizationCode(model, client),
                    "refresh_token" => throw new NotImplementedException(),
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

                if (!parts[1].StartsWith("Basic "))
                {
                    throw new SecurityException();
                }
                else
                {
                    return await _clientService.GetClient(parts[0], parts[1][6..]);
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

        private async Task<IActionResult> GetTokenForAuthorizationCode(TokenModel model, Client client)
        {
            var persistedGrant = await _grantService.GetGrant(model.Code, client.ClientId, model.CodeVerifier);

            // Validate return url if provided
            if (!_clientValidator.IsRedirectUriValid(client, model.RedirectUri))
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
