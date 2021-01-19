﻿using System;
using System.Collections.Generic;
using System.Text;
using Rinsen.DatabaseInstaller;

namespace Rinsen.InnovationBoost.Installation.IdentityServer
{
    public class OutbackTableInstallation : DatabaseVersion
    {
        public OutbackTableInstallation()
            : base(1)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var identityServerApiResourcesTable = dbChangeList.AddNewTable<IdentityServerApiResource>();
            identityServerApiResourcesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourcesTable.AddColumn(m => m.Description, 1000).Null();
            identityServerApiResourcesTable.AddColumn(m => m.DisplayName, 200).Null();
            identityServerApiResourcesTable.AddColumn(m => m.Enabled);
            identityServerApiResourcesTable.AddColumn(m => m.Name, 200).Unique("UX_IdentityServerApiResources_Name");
            identityServerApiResourcesTable.AddColumn(m => m.Created);
            identityServerApiResourcesTable.AddColumn(m => m.Updated);

            var identityServerApiResourceClaimsTable = dbChangeList.AddNewTable<IdentityServerApiResourceClaim>();
            identityServerApiResourceClaimsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourceClaimsTable.AddColumn(m => m.ApiResourceId).ForeignKey<IdentityServerApiResource>(m => m.Id);
            identityServerApiResourceClaimsTable.AddColumn(m => m.Type, 200);
            identityServerApiResourceClaimsTable.AddColumn(m => m.Created);
            identityServerApiResourceClaimsTable.AddColumn(m => m.Updated);

            var identityServerApiResourceScopesTable = dbChangeList.AddNewTable<IdentityServerApiResourceScope>();
            identityServerApiResourceScopesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourceScopesTable.AddColumn(m => m.ApiResourceId).ForeignKey<IdentityServerApiResource>(m => m.Id);
            identityServerApiResourceScopesTable.AddColumn(m => m.Description, 1000).Null();
            identityServerApiResourceScopesTable.AddColumn(m => m.DisplayName, 200).Null();
            identityServerApiResourceScopesTable.AddColumn(m => m.Emphasize);
            identityServerApiResourceScopesTable.AddColumn(m => m.Name, 200).Unique("UX_IdentityServerApiResourceScopes_Name"); ;
            identityServerApiResourceScopesTable.AddColumn(m => m.Required);
            identityServerApiResourceScopesTable.AddColumn(m => m.ShowInDiscoveryDocument);
            identityServerApiResourceScopesTable.AddColumn(m => m.Created);
            identityServerApiResourceScopesTable.AddColumn(m => m.Updated);

            var identityServerApiResourceScopeClaimsTable = dbChangeList.AddNewTable<IdentityServerApiResourceScopeClaim>();
            identityServerApiResourceScopeClaimsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourceScopeClaimsTable.AddColumn(m => m.ApiResourceScopeId).ForeignKey<IdentityServerApiResourceScope>(m => m.Id);
            identityServerApiResourceScopeClaimsTable.AddColumn(m => m.Type, 200);
            identityServerApiResourceScopeClaimsTable.AddColumn(m => m.Created);
            identityServerApiResourceScopeClaimsTable.AddColumn(m => m.Updated);

            var identityServerApiResourceSecretsTable = dbChangeList.AddNewTable<IdentityServerApiResourceSecret>();
            identityServerApiResourceSecretsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourceSecretsTable.AddColumn(m => m.ApiResourceId).ForeignKey<IdentityServerApiResource>(m => m.Id);
            identityServerApiResourceSecretsTable.AddColumn(m => m.Description, 1000).Null();
            identityServerApiResourceSecretsTable.AddColumn(m => m.Expiration);
            identityServerApiResourceSecretsTable.AddColumn(m => m.Type, 200);
            identityServerApiResourceSecretsTable.AddColumn(m => m.Value, 2000);
            identityServerApiResourceSecretsTable.AddColumn(m => m.Created);
            identityServerApiResourceSecretsTable.AddColumn(m => m.Updated);

            var identityServerApiResourcePropertiesTable = dbChangeList.AddNewTable<IdentityServerApiResourceProperty>("IdentityServerApiResourceProperties");
            identityServerApiResourcePropertiesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerApiResourcePropertiesTable.AddColumn(m => m.ApiResourceId).ForeignKey<IdentityServerApiResource>(m => m.Id);
            identityServerApiResourcePropertiesTable.AddColumn(m => m.Key, 250);
            identityServerApiResourcePropertiesTable.AddColumn(m => m.Value, 250);
            identityServerApiResourcePropertiesTable.AddColumn(m => m.Created);
            identityServerApiResourcePropertiesTable.AddColumn(m => m.Updated);

            var identityServerClientTypeTable = dbChangeList.AddNewTable<IdentityServerClientType>();
            identityServerClientTypeTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientTypeTable.AddColumn(m => m.Name, 256).Unique("UX_IdentityServerClientTypes_Name");
            identityServerClientTypeTable.AddColumn(m => m.Created);
            identityServerClientTypeTable.AddColumn(m => m.Updated);

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
            identityServerClientsTable.AddColumn(m => m.BackChannelLogoutUri, 2000).Null();
            identityServerClientsTable.AddColumn(m => m.ClientClaimsPrefix, 250);
            identityServerClientsTable.AddColumn(m => m.ClientName, 250);
            identityServerClientsTable.AddColumn(m => m.ClientUri, 2000).Null();
            identityServerClientsTable.AddColumn(m => m.ConsentLifetime);
            identityServerClientsTable.AddColumn(m => m.Description, 1000).Null();
            identityServerClientsTable.AddColumn(m => m.DeviceCodeLifetime);
            identityServerClientsTable.AddColumn(m => m.Enabled);
            identityServerClientsTable.AddColumn(m => m.EnableLocalLogin);
            identityServerClientsTable.AddColumn(m => m.FrontChannelLogoutSessionRequired);
            identityServerClientsTable.AddColumn(m => m.FrontChannelLogoutUri, 2000).Null();
            identityServerClientsTable.AddColumn(m => m.IdentityTokenLifetime);
            identityServerClientsTable.AddColumn(m => m.IncludeJwtId);
            identityServerClientsTable.AddColumn(m => m.LogoUri, 2000).Null();
            identityServerClientsTable.AddColumn(m => m.PairWiseSubjectSalt, 250).Null();
            identityServerClientsTable.AddColumn(m => m.ProtocolType, 250);
            identityServerClientsTable.AddColumn(m => m.RefreshTokenExpiration);
            identityServerClientsTable.AddColumn(m => m.RefreshTokenUsage);
            identityServerClientsTable.AddColumn(m => m.RequireClientSecret);
            identityServerClientsTable.AddColumn(m => m.RequireConsent);
            identityServerClientsTable.AddColumn(m => m.RequirePkce);
            identityServerClientsTable.AddColumn(m => m.SlidingRefreshTokenLifetime);
            identityServerClientsTable.AddColumn(m => m.UpdateAccessTokenClaimsOnRefresh);
            identityServerClientsTable.AddColumn(m => m.UserCodeType, 250).Null();
            identityServerClientsTable.AddColumn(m => m.UserSsoLifetime);
            identityServerClientsTable.AddColumn(m => m.ClientTypeId).Null().ForeignKey<IdentityServerClientType>(m => m.Id);
            identityServerClientsTable.AddColumn(m => m.Created);
            identityServerClientsTable.AddColumn(m => m.Updated);

            var identityServerClientClaimsTable = dbChangeList.AddNewTable<IdentityServerClientClaim>();
            identityServerClientClaimsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientClaimsTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientClaimsTable.AddColumn(m => m.Type, 250);
            identityServerClientClaimsTable.AddColumn(m => m.Value, 250);
            identityServerClientClaimsTable.AddColumn(m => m.Created);
            identityServerClientClaimsTable.AddColumn(m => m.Updated);

            var identityServerClientCorsTable = dbChangeList.AddNewTable<IdentityServerClientCorsOrigin>();
            identityServerClientCorsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientCorsTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientCorsTable.AddColumn(m => m.Origin, 2000);
            identityServerClientCorsTable.AddColumn(m => m.Created);
            identityServerClientCorsTable.AddColumn(m => m.Updated);

            var identityServerClientGrantTypesTable = dbChangeList.AddNewTable<IdentityServerClientGrantType>();
            identityServerClientGrantTypesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientGrantTypesTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientGrantTypesTable.AddColumn(m => m.GrantType, 250);
            identityServerClientGrantTypesTable.AddColumn(m => m.Created);
            identityServerClientGrantTypesTable.AddColumn(m => m.Updated);

            var identityServerClientIdpRestrictionsTable = dbChangeList.AddNewTable<IdentityServerClientIdpRestriction>();
            identityServerClientIdpRestrictionsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientIdpRestrictionsTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientIdpRestrictionsTable.AddColumn(m => m.Provider, 200);
            identityServerClientIdpRestrictionsTable.AddColumn(m => m.Created);
            identityServerClientIdpRestrictionsTable.AddColumn(m => m.Updated);

            var identityServerClientPostLogoutRedirectUrisTable = dbChangeList.AddNewTable<IdentityServerClientPostLogoutRedirectUri>();
            identityServerClientPostLogoutRedirectUrisTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientPostLogoutRedirectUrisTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientPostLogoutRedirectUrisTable.AddColumn(m => m.PostLogoutRedirectUri, 2000);
            identityServerClientPostLogoutRedirectUrisTable.AddColumn(m => m.Created);
            identityServerClientPostLogoutRedirectUrisTable.AddColumn(m => m.Updated);

            var identityServerClientRedirectUrisTable = dbChangeList.AddNewTable<IdentityServerClientRedirectUri>();
            identityServerClientRedirectUrisTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientRedirectUrisTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientRedirectUrisTable.AddColumn(m => m.RedirectUri, 2000);
            identityServerClientRedirectUrisTable.AddColumn(m => m.Created);
            identityServerClientRedirectUrisTable.AddColumn(m => m.Updated);

            var identityServerClientScopesTable = dbChangeList.AddNewTable<IdentityServerClientScope>();
            identityServerClientScopesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientScopesTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientScopesTable.AddColumn(m => m.Scope, 200);
            identityServerClientScopesTable.AddColumn(m => m.Created);
            identityServerClientScopesTable.AddColumn(m => m.Updated);

            var identityServerClientSecretsTable = dbChangeList.AddNewTable<IdentityServerClientSecret>();
            identityServerClientSecretsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerClientSecretsTable.AddColumn(m => m.ClientId).ForeignKey<IdentityServerClient>(m => m.Id);
            identityServerClientSecretsTable.AddColumn(m => m.Description, 1000).Null();
            identityServerClientSecretsTable.AddColumn(m => m.Expiration);
            identityServerClientSecretsTable.AddColumn(m => m.Type, 250).Null();
            identityServerClientSecretsTable.AddColumn(m => m.Value, 2000);
            identityServerClientSecretsTable.AddColumn(m => m.Created);
            identityServerClientSecretsTable.AddColumn(m => m.Updated);

            var identityServerIdentityResourcesTable = dbChangeList.AddNewTable<IdentityServerIdentityResource>();
            identityServerIdentityResourcesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerIdentityResourcesTable.AddColumn(m => m.Description, 1000).Null();
            identityServerIdentityResourcesTable.AddColumn(m => m.DisplayName, 200).Null();
            identityServerIdentityResourcesTable.AddColumn(m => m.Emphasize);
            identityServerIdentityResourcesTable.AddColumn(m => m.Enabled);
            identityServerIdentityResourcesTable.AddColumn(m => m.Name, 200).Unique("UX_IdentityServerIdentityResources_Name");
            identityServerIdentityResourcesTable.AddColumn(m => m.Required);
            identityServerIdentityResourcesTable.AddColumn(m => m.ShowInDiscoveryDocument);
            identityServerIdentityResourcesTable.AddColumn(m => m.Created);
            identityServerIdentityResourcesTable.AddColumn(m => m.Updated);

            var identityServerIdentityResourceClaimsTable = dbChangeList.AddNewTable<IdentityServerIdentityResourceClaim>();
            identityServerIdentityResourceClaimsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerIdentityResourceClaimsTable.AddColumn(m => m.IdentityResourceId).ForeignKey<IdentityServerIdentityResource>(m => m.Id);
            identityServerIdentityResourceClaimsTable.AddColumn(m => m.Type, 250);
            identityServerIdentityResourceClaimsTable.AddColumn(m => m.Created);
            identityServerIdentityResourceClaimsTable.AddColumn(m => m.Updated);

            var identityServerIdentityResourcePropertiesTable = dbChangeList.AddNewTable<IdentityServerIdentityResourceProperty>("IdentityServerIdentityResourceProperties");
            identityServerIdentityResourcePropertiesTable.AddAutoIncrementColumn(m => m.Id);
            identityServerIdentityResourcePropertiesTable.AddColumn(m => m.IdentityResourceId).ForeignKey<IdentityServerIdentityResource>(m => m.Id);
            identityServerIdentityResourcePropertiesTable.AddColumn(m => m.Key, 250);
            identityServerIdentityResourcePropertiesTable.AddColumn(m => m.Value, 250);
            identityServerIdentityResourcePropertiesTable.AddColumn(m => m.Created);
            identityServerIdentityResourcePropertiesTable.AddColumn(m => m.Updated);

            var identityServerPersistedGrantsTable = dbChangeList.AddNewTable<IdentityServerPersistedGrant>();
            identityServerPersistedGrantsTable.AddAutoIncrementColumn(m => m.Id);
            identityServerPersistedGrantsTable.AddColumn(m => m.ClientId, 200).ForeignKey<IdentityServerClient>(m => m.ClientId);
            identityServerPersistedGrantsTable.AddColumn(m => m.CreationTime);
            identityServerPersistedGrantsTable.AddColumn(m => m.Data, int.MaxValue);
            identityServerPersistedGrantsTable.AddColumn(m => m.Expiration);
            identityServerPersistedGrantsTable.AddColumn(m => m.Key, 250);
            identityServerPersistedGrantsTable.AddColumn(m => m.SubjectId, 250).Null();
            identityServerPersistedGrantsTable.AddColumn(m => m.Type, 250);

            var identityServerDeviceFlowCode = dbChangeList.AddNewTable<IdentityServerDeviceFlowCode>();
            identityServerDeviceFlowCode.AddAutoIncrementColumn(m => m.Id);
            identityServerDeviceFlowCode.AddColumn(m => m.DeviceCode, length: 200).Unique("UX_IdentityServerDeviceFlowCodes_DeviceCode");
            identityServerDeviceFlowCode.AddColumn(m => m.UserCode, length: 200).Unique("UX_IdentityServerDeviceFlowCodes_UserCode"); ;
            identityServerDeviceFlowCode.AddColumn(m => m.ClientId, length: 200).ForeignKey<IdentityServerClient>(m => m.ClientId);
            identityServerDeviceFlowCode.AddColumn(m => m.CreationTime);
            identityServerDeviceFlowCode.AddColumn(m => m.Expiration);
            identityServerDeviceFlowCode.AddColumn(m => m.IdentityId).ForeignKey("Identities", "IdentityId");
            identityServerDeviceFlowCode.AddColumn(m => m.SerializedDeviceFlowCode, int.MaxValue);

        }
    }
}