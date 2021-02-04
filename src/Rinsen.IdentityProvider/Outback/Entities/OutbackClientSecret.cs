using System;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackClientSecret : ICreatedAndUpdatedTimestamp, ISoftDelete
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string Secret { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Deleted { get; set; }

        public virtual OutbackClient Client { get; set; }
    }
}
