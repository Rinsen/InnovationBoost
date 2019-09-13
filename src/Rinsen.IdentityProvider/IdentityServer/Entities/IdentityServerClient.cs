using System;
using System.Collections.Generic;
using IdentityServer4.Models;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerClient : ICreatedAndUpdatedTimestamp, IObjectWithState
    {
        public bool AllowOfflineAccess { get; set; }
        
        public int IdentityTokenLifetime { get; set; }
        
        public int AccessTokenLifetime { get; set; }
        
        public int AuthorizationCodeLifetime { get; set; }
        
        public int AbsoluteRefreshTokenLifetime { get; set; }
        
        public int SlidingRefreshTokenLifetime { get; set; }
        
        public TokenUsage RefreshTokenUsage { get; set; }
        
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        
        public TokenExpiration RefreshTokenExpiration { get; set; }
        
        public AccessTokenType AccessTokenType { get; set; }
        
        public bool EnableLocalLogin { get; set; }
        
        public bool IncludeJwtId { get; set; }
        
        public bool AlwaysSendClientClaims { get; set; }
        
        public string ClientClaimsPrefix { get; set; }
        
        public string PairWiseSubjectSalt { get; set; }
        
        public int? UserSsoLifetime { get; set; }
        
        public string UserCodeType { get; set; }
        
        public int DeviceCodeLifetime { get; set; }
        
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        
        public bool BackChannelLogoutSessionRequired { get; set; }
        
        public bool Enabled { get; set; }
        
        public string ClientId { get; set; }

        public int Id { get; set; }

        public string ProtocolType { get; set; }
        
        public bool RequireClientSecret { get; set; }
        
        public string ClientName { get; set; }
        
        public string Description { get; set; }
        
        public string ClientUri { get; set; }
        
        public string LogoUri { get; set; }
        
        public bool RequireConsent { get; set; }

        public int? ConsentLifetime { get; set; }

        public bool RequirePkce { get; set; }
        
        public bool AllowPlainTextPkce { get; set; }
        
        public bool AllowAccessTokensViaBrowser { get; set; }
        
        public string FrontChannelLogoutUri { get; set; }
        
        public bool FrontChannelLogoutSessionRequired { get; set; }
        
        public string BackChannelLogoutUri { get; set; }
        
        public bool AllowRememberConsent { get; set; }

        public int? ClientTypeId { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public ObjectState State { get; set; }

        public List<IdentityServerClientCorsOrigin> AllowedCorsOrigins { get; set; }

        public List<IdentityServerClientGrantType> AllowedGrantTypes { get; set; }

        public List<IdentityServerClientScope> AllowedScopes { get; set; }

        public List<IdentityServerClientClaim> Claims { get; set; }

        public List<IdentityServerClientSecret> ClientSecrets { get; set; }

        public List<IdentityServerClientIdpRestriction> IdentityProviderRestrictions { get; set; }

        public List<IdentityServerClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }

        public List<IdentityServerClientRedirectUri> RedirectUris { get; set; }
    }
}
