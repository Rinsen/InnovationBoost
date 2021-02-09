using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Rinsen.Outback.Clients;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackClient : ICreatedAndUpdatedTimestamp, ISoftDelete
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public ClientType ClientType { get; set; }
        public int ClientFamilyId { get; set; }
        public bool ConsentRequired { get; set; } = false;
        public bool SaveConsent { get; set; } = false;

        /// <summary>
        /// Default 30 days
        /// </summary>
        public int SavedConsentLifetime { get; set; } = 2592000;

        public bool IssueRefreshToken { get; set; } = false;

        /// <summary>
        /// Default 30 days
        /// </summary>
        public int RefreshTokenLifetime { get; set; } = 2592000;

        /// <summary>
        /// Default 1 hour
        /// </summary>
        public int AccessTokenLifetime { get; set; } = 3600;

        /// <summary>
        /// Default 10 minutes
        /// </summary>
        public int IdentityTokenLifetime { get; set; } = 600;

        /// <summary>
        /// Default 10 minutes
        /// </summary>
        public int AuthorityCodeLifetime { get; set; } = 600;

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

        [JsonIgnore]
        public List<OutbackCodeGrant> CodeGrants { get; set; }

        [JsonIgnore]
        public List<OutbackPersistedGrant> PersistedGrants { get; set; }

        [JsonIgnore]
        public List<OutbackRefreshTokenGrant> RefreshTokenGrants { get; set; }
    }
}
