using System;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackPersistedGrant
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Guid SubjectId { get; set; }

        public string Scope { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }

        public virtual OutbackClient Client { get; set; }


    }
}
