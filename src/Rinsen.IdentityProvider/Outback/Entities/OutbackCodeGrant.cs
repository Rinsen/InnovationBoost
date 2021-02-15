using System;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackCodeGrant
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Guid SubjectId { get; set; }

        public string Code { get; set; }

        public string CodeChallange { get; set; }

        public string CodeChallangeMethod { get; set; }

        public string State { get; set; }

        public string Nonce { get; set; }

        public string RedirectUri { get; set; }

        public string Scope { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Resolved { get; set; }

        public DateTime Expires { get; set; }

        public virtual OutbackClient Client { get; set; }
    }
}
