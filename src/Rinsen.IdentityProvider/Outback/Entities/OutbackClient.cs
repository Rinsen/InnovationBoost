using System;
using System.Collections.Generic;
using Rinsen.Outback.Clients;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackClient : ICreatedAndUpdatedTimestamp, ISoftDelete
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public ClientType ClientType { get; set; }
        public int ClientFamilyId { get; set; }
        public bool ConsentRequired { get; set; }
        public bool SaveConsent { get; set; }
        public bool SavedConsentLifetime { get; set; }
        public bool IssueRefreshToken { get; set; }
        public int RefreshTokenLifetime { get; set; }
        public int AccessTokenLifetime { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public int AuthorityCodeLifetime { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Deleted { get; set; }

        public virtual OutbackClientFamily ClientFamily { get; set; }

        public List<OutbackClientClaim> ClientClaims { get; set; }

        public List<OutbackClientSecret> Secrets { get; set; }

        public List<OutbackClientScope> Scopes { get; set; }

        public List<OutbackClientSupportedGrantType> SupportedGrantTypes { get; set; }

        public List<OutbackClientLoginRedirectUri> LoginRedirectUris { get; set; }

        public List<OutbackClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }

        public List<OutbackAllowedCorsOrigin> AllowedCorsOrigins { get; set; }

        public List<OutbackCodeGrant> CodeGrants { get; set; }

        public List<OutbackPersistedGrant> PersistedGrants { get; set; }

        public List<OutbackRefreshTokenGrant> RefreshTokenGrants { get; set; }
    }
}
