using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider.IdentityServer;
using Rinsen.IdentityProvider.IdentityServer.Entities;
using Rinsen.InnovationBoost.ApiModels;
using Rinsen.InnovationBoost.Models;

namespace Rinsen.InnovationBoost.ApiControllers
{
    [ApiController]
    [Route("IdentityServer/api/[controller]")]
    [Authorize("AdminsOnly")]
    public class ClientController : Controller
    {
        private readonly IdentityServerClientBusiness _identityServerClientBusiness;
        private readonly IdentityServerApiResourceBusiness _identityServerApiResourceBusiness;
        private readonly IdentityServerIdentityResourceBusiness _identityServerIdentityResourceBusiness;

        public ClientController(IdentityServerClientBusiness identityServerClientBusiness,
            IdentityServerApiResourceBusiness identityServerApiResourceBusiness,
            IdentityServerIdentityResourceBusiness identityServerIdentityResourceBusiness)
        {
            _identityServerClientBusiness = identityServerClientBusiness;
            _identityServerApiResourceBusiness = identityServerApiResourceBusiness;
            _identityServerIdentityResourceBusiness = identityServerIdentityResourceBusiness;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<List<IdentityServerClientModel>>> GetAll()
        {
            var identityServerClients = await _identityServerClientBusiness.GetIdentityServerClients();
            var identityServerApiResources = await _identityServerApiResourceBusiness.GetIdentityServerApiResourcesAsync();
            var identityServerIdentityResources = await _identityServerIdentityResourceBusiness.GetIdentityServerIdentityResourcesAsync();

            identityServerClients.ForEach(c => c.ClientSecrets.ForEach(s => s.Value = "****"));

            var identityServerClientModels = new List<IdentityServerClientModel>();
            identityServerClients.ForEach(client => identityServerClientModels.Add(MapIdentityServerClientToModel(client, identityServerApiResources, identityServerIdentityResources)));

            return identityServerClientModels;
        }

        [HttpGet("{id}", Name = "GetClient")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IdentityServerClientModel>> GetById(string id)
        {
            var identityServerClient = await _identityServerClientBusiness.GetIdentityServerClient(id);

            if (identityServerClient == default(IdentityServerClient))
                return NotFound();

            identityServerClient.ClientSecrets.ForEach(s => s.Value = "****");

            var identityServerApiResources = await _identityServerApiResourceBusiness.GetIdentityServerApiResourcesAsync();
            var identityServerIdentityResources = await _identityServerIdentityResourceBusiness.GetIdentityServerIdentityResourcesAsync();

            var identityServerClientModel = MapIdentityServerClientToModel(identityServerClient, identityServerApiResources, identityServerIdentityResources);

            return identityServerClientModel;
        }

        [HttpDelete("{id}", Name = "DeleteClient")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(string id)
        {
            await _identityServerClientBusiness.DeleteIdentityServerClient(id);

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Route("~/IdentityServer/api/[controller]/Type")]
        public async Task<ActionResult<List<IdentityServerClientType>>> GetAllClientTypes()
        {
            var identityServerClientTypes = await _identityServerClientBusiness.GetAllClientTypes();

            return identityServerClientTypes;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/Create")]
        public async Task<ActionResult<IdentityServerClientModel>> Create([Required]CreateClient createClient)
        {
            await _identityServerClientBusiness.CreateNewClient(createClient.ClientId, createClient.ClientName, createClient.Description);

            var client = await _identityServerClientBusiness.GetIdentityServerClient(createClient.ClientId);

            return CreatedAtAction(nameof(GetById),
               new { id = createClient.ClientId }, client);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/CreateNode")]
        public async Task<ActionResult<IdentityServerClientModel>> CreateNode([Required]CreateTypedClient createClient)
        {
            var clientId = await _identityServerClientBusiness.CreateNewTypedClient(createClient.ClientName, createClient.Description, "Node");

            var client = await _identityServerClientBusiness.GetIdentityServerClient(clientId);

            return CreatedAtAction(nameof(GetById),
               new { id = clientId }, client);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/CreateWebsite")]
        public async Task<ActionResult<IdentityServerClientModel>> CreateWebsite([Required]CreateTypedClient createClient)
        {
            var clientId = await _identityServerClientBusiness.CreateNewTypedClient(createClient.ClientName, createClient.Description, "Website");

            var client = await _identityServerClientBusiness.GetIdentityServerClient(clientId);

            return CreatedAtAction(nameof(GetById),
               new { id = clientId }, client);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/Update")]
        public async Task<ActionResult> Update([Required]IdentityServerClientModel identityServerClient)
        {
            AddAllowedScopesFromSelectedResources(identityServerClient);

            await _identityServerClientBusiness.UpdateClient(identityServerClient);

            return Ok();
        }

        private static void AddAllowedScopesFromSelectedResources(IdentityServerClientModel identityServerClient)
        {
            var selectedScopes = new List<string>();

            foreach (var apiResourceModel in identityServerClient.IdentityServerApiResources)
            {
                if (apiResourceModel.Selected)
                {
                    selectedScopes.Add(apiResourceModel.Name);
                }

                foreach (var scopeModel in apiResourceModel.IdentityServerApiResourceScopes)
                {
                    if (scopeModel.Selected)
                    {
                        selectedScopes.Add(scopeModel.Name);
                    }
                }
            }

            foreach (var identityResourceModel in identityServerClient.IdentityServerIdentityResources)
            {
                if (identityResourceModel.Selected)
                {
                    selectedScopes.Add(identityResourceModel.Name);
                }
            }

            var scopesToRemove = identityServerClient.AllowedScopes.Select(m => m.Scope).Except(selectedScopes).ToArray();
            var scopesToAdd = selectedScopes.Except(identityServerClient.AllowedScopes.Select(m => m.Scope)).ToArray();

            foreach (var scopeName in scopesToAdd)
            {
                identityServerClient.AllowedScopes.Add(new IdentityServerClientScope { Scope = scopeName, State = ObjectState.Added });
            }

            foreach (var scopeName in scopesToRemove)
            {
                identityServerClient.AllowedScopes.Single(m => m.Scope == scopeName).State = ObjectState.Deleted;
            }
        }  

        private IdentityServerClientModel MapIdentityServerClientToModel(IdentityServerClient identityServerClient, List<IdentityServerApiResource> identityServerApiResources, List<IdentityServerIdentityResource> identityServerIdentityResources)
        {
            var identityServerClientModel = new IdentityServerClientModel
            {
                AbsoluteRefreshTokenLifetime = identityServerClient.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = identityServerClient.AccessTokenLifetime,
                AccessTokenType = identityServerClient.AccessTokenType,
                AllowAccessTokensViaBrowser = identityServerClient.AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = new List<IdentityServerClientCorsOrigin>(),
                AllowedGrantTypes = new List<IdentityServerClientGrantType>(),
                AllowedScopes = new List<IdentityServerClientScope>(),
                AllowOfflineAccess = identityServerClient.AllowOfflineAccess,
                AllowPlainTextPkce = identityServerClient.AllowPlainTextPkce,
                AllowRememberConsent = identityServerClient.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = identityServerClient.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = identityServerClient.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = identityServerClient.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = identityServerClient.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = identityServerClient.BackChannelLogoutUri,
                Claims = new List<IdentityServerClientClaim>(),
                ClientClaimsPrefix = identityServerClient.ClientClaimsPrefix,
                ClientId = identityServerClient.ClientId,
                ClientName = identityServerClient.ClientName,
                ClientSecrets = new List<IdentityServerClientSecret>(),
                ClientTypeId = identityServerClient.ClientTypeId,
                ClientUri = identityServerClient.ClientUri,
                ConsentLifetime = identityServerClient.ConsentLifetime,
                Created = identityServerClient.Created,
                Description = identityServerClient.Description,
                DeviceCodeLifetime = identityServerClient.DeviceCodeLifetime,
                Enabled = identityServerClient.Enabled,
                EnableLocalLogin = identityServerClient.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = identityServerClient.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = identityServerClient.FrontChannelLogoutUri,
                Id = identityServerClient.Id,
                IdentityProviderRestrictions = new List<IdentityServerClientIdpRestriction>(),
                IdentityTokenLifetime = identityServerClient.IdentityTokenLifetime,
                IncludeJwtId = identityServerClient.IncludeJwtId,
                LogoUri = identityServerClient.LogoUri,
                PairWiseSubjectSalt = identityServerClient.PairWiseSubjectSalt,
                PostLogoutRedirectUris = new List<IdentityServerClientPostLogoutRedirectUri>(),
                ProtocolType = identityServerClient.ProtocolType,
                RedirectUris = new List<IdentityServerClientRedirectUri>(),
                RefreshTokenExpiration = identityServerClient.RefreshTokenExpiration,
                RefreshTokenUsage = identityServerClient.RefreshTokenUsage,
                RequireClientSecret = identityServerClient.RequireClientSecret,
                RequireConsent = identityServerClient.RequireConsent,
                RequirePkce = identityServerClient.RequirePkce,
                SlidingRefreshTokenLifetime = identityServerClient.SlidingRefreshTokenLifetime,
                Updated = identityServerClient.Updated,
                UpdateAccessTokenClaimsOnRefresh = identityServerClient.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = identityServerClient.UserCodeType,
                UserSsoLifetime = identityServerClient.UserSsoLifetime
            };

            identityServerClientModel.AllowedCorsOrigins = identityServerClient.AllowedCorsOrigins;
            identityServerClientModel.AllowedGrantTypes = identityServerClient.AllowedGrantTypes;
            identityServerClientModel.AllowedScopes = identityServerClient.AllowedScopes;
            identityServerClientModel.Claims = identityServerClient.Claims;
            identityServerClientModel.ClientSecrets = identityServerClient.ClientSecrets;
            identityServerClientModel.IdentityProviderRestrictions = identityServerClient.IdentityProviderRestrictions;
            identityServerClientModel.PostLogoutRedirectUris = identityServerClient.PostLogoutRedirectUris;

            foreach (var apiResource in identityServerApiResources)
            {
                var identityServerApiResourceModel = new IdentityServerApiResourceModel
                {
                    Selected = IsSelected(apiResource.Name, identityServerClientModel.AllowedScopes),
                    Description = apiResource.Description,
                    DisplayName = apiResource.DisplayName,
                    Name = apiResource.Name,
                    Enabled = apiResource.Enabled,
                    Id = apiResource.Id
                };

                identityServerClientModel.IdentityServerApiResources.Add(identityServerApiResourceModel);

                foreach (var scope in apiResource.Scopes)
                {
                    var identityServerApiResourceScopeModel = new IdentityServerApiResourceScopeModel
                    {
                        Selected = IsSelected(scope.Name, identityServerClientModel.AllowedScopes),
                        Description = scope.Description,
                        DisplayName = scope.DisplayName,
                        Name = scope.Name
                    };

                    identityServerApiResourceModel.IdentityServerApiResourceScopes.Add(identityServerApiResourceScopeModel);
                }
            }

            foreach (var identityResource in identityServerIdentityResources)
            { 
                var identityServerIdentityResourceModel = new IdentityServerIdentityResourceModel
                {
                    Selected = IsSelected(identityResource.Name, identityServerClientModel.AllowedScopes),
                    Description = identityResource.Description,
                    DisplayName = identityResource.DisplayName,
                    Name = identityResource.Name,
                    Enabled = identityResource.Enabled,
                    Id = identityResource.Id
                };

                identityServerClientModel.IdentityServerIdentityResources.Add(identityServerIdentityResourceModel);
            }
            
            return identityServerClientModel;
        }

        private bool IsSelected(string name, List<IdentityServerClientScope> allowedScopes)
        {
            return allowedScopes.Any(m => m.Scope == name);
        }
    }
}
