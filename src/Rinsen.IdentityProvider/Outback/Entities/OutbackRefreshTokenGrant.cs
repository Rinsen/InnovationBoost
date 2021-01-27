using System;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackRefreshTokenGrant
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string SubjectId { get; set; }

        public string RefreshToken { get; set; }

        public string Scope { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Resolved { get; set; }

        public DateTime Expires { get; set; }

        public virtual OutbackClient Client { get; set; }
    }
}
