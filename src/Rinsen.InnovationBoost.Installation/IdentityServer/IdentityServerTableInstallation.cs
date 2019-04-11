using System;
using System.Collections.Generic;
using System.Text;
using Rinsen.DatabaseInstaller;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.IdentityProvider.Installation.IdentityServer
{
    public class IdentityServerTableInstallation : DatabaseVersion
    {
        // https://id4withclients.readthedocs.io/en/latest/id4/ID4Database/DatabaseDiagramID4.html

        public IdentityServerTableInstallation() 
            : base(1)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var identityServerApiResourcesTable = dbChangeList.AddNewTable<IdentityServerApiResource>();

            identityServerApiResourcesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourcesTable.AddColumn(m => m.Description, 1000);
            identityServerApiResourcesTable.AddColumn(m => m.DisplayName, 200);
            identityServerApiResourcesTable.AddColumn(m => m.Enabled);
            identityServerApiResourcesTable.AddColumn(m => m.Name, 200);

            var identityServerApiResourceClaimsTable = dbChangeList.AddNewTable<IdentityServerApiResourceClaim>();
            identityServerApiResourceClaimsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourceClaimsTable.AddColumn(m => m.ApiResourceId).ForeignKey<IdentityServerApiResource>(m => m.Id);
            identityServerApiResourceClaimsTable.AddColumn(m => m.Type, 200);

            var identityServerApiResourceScopesTable = dbChangeList.AddNewTable<IdentityServerApiResourceScope>();
            identityServerApiResourceScopesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourceScopesTable.AddColumn(m => m.ApiResourceId).ForeignKey<IdentityServerApiResource>(m => m.Id);
            identityServerApiResourceScopesTable.AddColumn(m => m.Description, 1000);
            identityServerApiResourceScopesTable.AddColumn(m => m.DisplayName, 200);
            identityServerApiResourceScopesTable.AddColumn(m => m.Emphasize);
            identityServerApiResourceScopesTable.AddColumn(m => m.Name, 200);
            identityServerApiResourceScopesTable.AddColumn(m => m.Required);
            identityServerApiResourceScopesTable.AddColumn(m => m.ShowInDiscoveryDocument);

            var identityServerApiResourceScopeClaimsTable = dbChangeList.AddNewTable<IdentityServerApiResourceScopeClaim>();
            identityServerApiResourceScopeClaimsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourceScopeClaimsTable.AddColumn(m => m.ApiResourceScopeId).ForeignKey<IdentityServerApiResourceScope>(m => m.Id);
            identityServerApiResourceScopeClaimsTable.AddColumn(m => m.Type, 200);

            var identityServerApiResourceSecretsTable = dbChangeList.AddNewTable<IdentityServerApiResourceSecret>();
            identityServerApiResourceSecretsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourceSecretsTable.AddColumn(m => m.ApiResourceId).ForeignKey<IdentityServerApiResource>(m => m.Id);
            identityServerApiResourceSecretsTable.AddColumn(m => m.Description, 1000);
            identityServerApiResourceSecretsTable.AddColumn(m => m.Expiration);
            identityServerApiResourceSecretsTable.AddColumn(m => m.Type, 200);
            identityServerApiResourceSecretsTable.AddColumn(m => m.Value, 2000);

            var identityServerClientsTable = dbChangeList.AddNewTable<IdentityServerClient>();
            identityServerClientsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientsTable.AddColumn(m => m.ClientId, 200).Unique("UX_IdentityServerClients_ClientId");
            identityServerClientsTable.AddColumn(m => m.AbsoluteRefreshTokenLifetime);
            identityServerClientsTable.AddColumn(m => m.AccessTokenLifetime);
            identityServerClientsTable.AddColumn(m => m.AccessTokenType);
            identityServerClientsTable.AddColumn(m => m.AllowAccessTokensViaBrowser);
            identityServerClientsTable.AddColumn(m => m.AllowOfflineAccess);
            identityServerClientsTable.AddColumn(m => m.AllowPlainTextPkce);
            identityServerClientsTable.AddColumn(m => m.AllowRememberConsent);
            identityServerClientsTable.AddColumn(m => m.AlwaysIncludeUserClaimsInIdToken);
            identityServerClientsTable.AddColumn(m => m.AlwaysSendClientClaims);
            identityServerClientsTable.AddColumn(m => m.AuthorizationCodeLifetime);
            identityServerClientsTable.AddColumn(m => m.BackChannelLogoutSessionRequired);
            identityServerClientsTable.AddColumn(m => m.BackChannelLogoutUri, 2000);
            identityServerClientsTable.AddColumn(m => m.ClientClaimsPrefix, 250);
            identityServerClientsTable.AddColumn(m => m.ClientName, 250);
            identityServerClientsTable.AddColumn(m => m.ClientUri, 2000);
            identityServerClientsTable.AddColumn(m => m.Description, 1000);
            identityServerClientsTable.AddColumn(m => m.DeviceCodeLifetime);
            identityServerClientsTable.AddColumn(m => m.Enabled);
            identityServerClientsTable.AddColumn(m => m.EnableLocalLogin);
            identityServerClientsTable.AddColumn(m => m.FrontChannelLogoutSessionRequired);
            identityServerClientsTable.AddColumn(m => m.FrontChannelLogoutUri, 2000);
            identityServerClientsTable.AddColumn(m => m.IdentityTokenLifetime);
            identityServerClientsTable.AddColumn(m => m.IncludeJwtId);
            identityServerClientsTable.AddColumn(m => m.LogoUri, 2000);
            identityServerClientsTable.AddColumn(m => m.PairWiseSubjectSalt, 250);
            identityServerClientsTable.AddColumn(m => m.ProtocolType, 250);
            identityServerClientsTable.AddColumn(m => m.RefreshTokenExpiration);
            identityServerClientsTable.AddColumn(m => m.RefreshTokenUsage);
            identityServerClientsTable.AddColumn(m => m.RequireClientSecret);
            identityServerClientsTable.AddColumn(m => m.RequireConsent);
            identityServerClientsTable.AddColumn(m => m.RequirePkce);
            identityServerClientsTable.AddColumn(m => m.SlidingRefreshTokenLifetime);
            identityServerClientsTable.AddColumn(m => m.UpdateAccessTokenClaimsOnRefresh);
            identityServerClientsTable.AddColumn(m => m.UserCodeType, 250);
            identityServerClientsTable.AddColumn(m => m.UserSsoLifetime);
            // And a lot more...

            var identityServerClientClaimsTable = dbChangeList.AddNewTable<IdentityServerClientClaim>();
            identityServerClientClaimsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientClaimsTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientClaimsTable.AddColumn(m => m.Type, 250);
            identityServerClientClaimsTable.AddColumn(m => m.Value, 250);

            var identityServerClientGrantTypesTable = dbChangeList.AddNewTable<IdentityServerClientGrantType>();
            identityServerClientGrantTypesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientGrantTypesTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientGrantTypesTable.AddColumn(m => m.GrantType, 250);

            var identityServerClientIdpRestrictionsTable = dbChangeList.AddNewTable<IdentityServerClientIdpRestriction>();
            identityServerClientIdpRestrictionsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientIdpRestrictionsTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientIdpRestrictionsTable.AddColumn(m => m.Provider, 200);

            var identityServerClientPostLogoutRedirectUrisTable = dbChangeList.AddNewTable<IdentityServerClientPostLogoutRedirectUri>();
            identityServerClientPostLogoutRedirectUrisTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientPostLogoutRedirectUrisTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientPostLogoutRedirectUrisTable.AddColumn(m => m.PostLogoutRedirectUri, 2000);

            var identityServerClientRedirectUrisTable = dbChangeList.AddNewTable<IdentityServerClientRedirectUri>();
            identityServerClientRedirectUrisTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientRedirectUrisTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientRedirectUrisTable.AddColumn(m => m.RedirectUri, 2000);

            var identityServerClientScopesTable = dbChangeList.AddNewTable<IdentityServerClientScope>();
            identityServerClientScopesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientScopesTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientScopesTable.AddColumn(m => m.Scope, 200);

            var identityServerClientSecretsTable = dbChangeList.AddNewTable<IdentityServerClientSecret>();
            identityServerClientSecretsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientSecretsTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientSecretsTable.AddColumn(m => m.Description, 1000);
            identityServerClientSecretsTable.AddColumn(m => m.Expiration);
            identityServerClientSecretsTable.AddColumn(m => m.Type, 250);
            identityServerClientSecretsTable.AddColumn(m => m.Value, 2000);

            var identityServerIdentityResourcesTable = dbChangeList.AddNewTable<IdentityServerIdentityResource>();
            identityServerIdentityResourcesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerIdentityResourcesTable.AddColumn(m => m.Description, 1000);
            identityServerIdentityResourcesTable.AddColumn(m => m.DisplayName, 200);
            identityServerIdentityResourcesTable.AddColumn(m => m.Emphasize);
            identityServerIdentityResourcesTable.AddColumn(m => m.Enabled);
            identityServerIdentityResourcesTable.AddColumn(m => m.Name, 200);
            identityServerIdentityResourcesTable.AddColumn(m => m.Required);
            identityServerIdentityResourcesTable.AddColumn(m => m.ShowInDiscoveryDocument);

            var identityServerIdentityResourceClaimsTable = dbChangeList.AddNewTable<IdentityServerIdentityResourceClaim>();
            identityServerIdentityResourceClaimsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerIdentityResourceClaimsTable.AddColumn(m => m.IdentityResourceId).ForeignKey<IdentityServerIdentityResource>(m => m.Id);
            identityServerIdentityResourceClaimsTable.AddColumn(m => m.Type, 250);

            var identityServerIdentityResourcePropertiesTable = dbChangeList.AddNewTable<IdentityServerIdentityResourceProperty>();
            identityServerIdentityResourcePropertiesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerIdentityResourcePropertiesTable.AddColumn(m => m.IdentityResourceId).ForeignKey<IdentityServerIdentityResource>(m => m.Id);
            identityServerIdentityResourcePropertiesTable.AddColumn(m => m.Key, 250);
            identityServerIdentityResourcePropertiesTable.AddColumn(m => m.Value, 250);

            var identityServerPersistedGrantsTable = dbChangeList.AddNewTable<IdentityServerPersistedGrant>();
            identityServerPersistedGrantsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerPersistedGrantsTable.AddColumn(m => m.ClientId, 200).ForeignKey<IdentityServerClient>(m => m.ClientId);
            identityServerPersistedGrantsTable.AddColumn(m => m.CreationTime);
            identityServerPersistedGrantsTable.AddColumn(m => m.Data);
            identityServerPersistedGrantsTable.AddColumn(m => m.Expiration);
            identityServerPersistedGrantsTable.AddColumn(m => m.Key, 250);
            identityServerPersistedGrantsTable.AddColumn(m => m.SubjectId, 250);
            identityServerPersistedGrantsTable.AddColumn(m => m.Type, 250);

        }
    }
}
