using System;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackClientSupportedGrantType : ICreatedAndUpdatedTimestamp, ISoftDelete
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string GrantType { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Deleted { get; set; }

        public virtual OutbackClient Client { get; set; }
    }
}
