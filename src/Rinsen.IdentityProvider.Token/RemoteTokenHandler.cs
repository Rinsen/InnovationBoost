using Microsoft.AspNetCore.Authentication;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Rinsen.IdentityProvider.Contracts.v1;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Linq;
using Rinsen.IdentityProvider.Contracts;

namespace Rinsen.IdentityProvider.Token
{
    public class RemoteTokenHandler : RemoteAuthenticationHandler<TokenOptions>
    {
        public RemoteTokenHandler(IOptionsMonitor<TokenOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            // Only run this if user is not authenticated
            if (Context.User.Identity.IsAuthenticated)
            {
                return HandleRequestResult.SkipHandler();
            }

            string authorizationToken = Request.Query["AuthToken"];

            if (string.IsNullOrEmpty(authorizationToken))
            {
                return HandleRequestResult.Fail("No authorization header.");
            }

            if (string.IsNullOrEmpty(Options.ApplicationKey))
            {
                throw new InvalidOperationException("No application key is provided");
            }
            try
            {
                var validationUrl = Options.IdentityServiceUrl + "/api/v1/Identity/Get" +
                        QueryString.Create(new[]
                        {
                            new KeyValuePair<string, string>(Options.TokenParameterName, authorizationToken),
                            new KeyValuePair<string, string>(Options.ApplicationKeyParameterName, Options.ApplicationKey)
                        }).ToUriComponent();

                var request = new HttpRequestMessage(HttpMethod.Get, validationUrl);

                var response = await Options.Backchannel.SendAsync(request, Context.RequestAborted);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"An error occurred when retrieving user information ({response.StatusCode}). Please check if the authentication information is correct and the corresponding RinsenIdentity API is enabled.");
                }

                var externalIdentity = JsonConvert.DeserializeObject<ExternalIdentity>(await response.Content.ReadAsStringAsync());

                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, externalIdentity.IdentityId.ToString(), nameof(Guid), externalIdentity.Issuer),
                            new Claim(ClaimTypes.Name, $"{externalIdentity.GivenName} {externalIdentity.Surname}", externalIdentity.Issuer),
                            new Claim(ClaimTypes.Email, externalIdentity.Email, externalIdentity.Issuer),
                            new Claim(ClaimTypes.MobilePhone, externalIdentity.PhoneNumber, externalIdentity.Issuer),
                            new Claim(ClaimTypes.GivenName, externalIdentity.GivenName, externalIdentity.Issuer),
                            new Claim(ClaimTypes.Surname, externalIdentity.Surname, externalIdentity.Issuer),
                            new Claim(ClaimTypes.Expiration, externalIdentity.Expiration.ToString(), externalIdentity.Issuer),
                            new Claim(ClaimTypes.SerialNumber, externalIdentity.CorrelationId.ToString(), externalIdentity.Issuer)
                        };

                if (externalIdentity.Extensions.Any(c => c.Type == RinsenIdentityConstants.Role && c.Value == RinsenIdentityConstants.Administrator))
                {
                    claims.Add(new Claim(ClaimTypes.Role, RinsenIdentityConstants.Administrator, externalIdentity.Issuer));
                }

                var claimsIdentiy = new ClaimsIdentity(claims, Options.ClaimsIssuer);

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentiy);
                var authTicket = new AuthenticationTicket(claimsPrincipal, new AuthenticationProperties { IsPersistent = externalIdentity.Expiration }, TokenDefaults.AuthenticationScheme);
                return HandleRequestResult.Success(authTicket);
                
            }
            catch (Exception e)
            {
                Logger.LogError(1, e, $"Validate token {authorizationToken} failed");
                return HandleRequestResult.Fail(e);
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (Request.Query.ContainsKey("AuthToken"))
            {
                throw new InvalidOperationException("Possible auth loop detected");
            }

            var externalUrl = Options.CallbackPath;// OriginalPathBase + Request.Path + Request.QueryString; What to do with this?!

            var loginUri = Options.IdentityServiceUrl + "/Identity/Login" + QueryString.Create(new[]
                        {
                                    new KeyValuePair<string, string>(Options.ExternalUrlParamterName, externalUrl),
                                    new KeyValuePair<string, string>(Options.HostParameterName, Request.Host.Value),
                                    new KeyValuePair<string, string>(Options.ApplicationNameParameterName, Options.ApplicationName)
                                }).ToUriComponent();

            if (IsAjaxRequest(Request))
            {
                Response.Headers["Location"] = loginUri;
                Response.StatusCode = 401;
            }
            else
            {
                Response.Redirect(loginUri);
            }

            return Task.CompletedTask;
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
                string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);
        }
    }
}
