using System;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackScope : ICreatedAndUpdatedTimestamp, ISoftDelete
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ScopeName { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Deleted { get; set; }
    }
}
