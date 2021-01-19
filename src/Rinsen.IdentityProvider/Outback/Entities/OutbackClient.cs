using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rinsen.Outback.Clients;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackClient : ICreatedAndUpdatedTimestamp, ISoftDelete
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public ClientType ClientType { get; set; }
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

        public List<ClientClaim> ClientClaims { get; set; }

        public List<string> Secrets { get; set; }

        public List<string> Scopes { get; set; }

        public List<string> SupportedGrantTypes { get; set; }

        public List<string> LoginRedirectUris { get; set; }

        public List<string> PostLogoutRedirectUris { get; set; }

        public List<string> AllowedCorsOrigins { get; set; }
    }
}
