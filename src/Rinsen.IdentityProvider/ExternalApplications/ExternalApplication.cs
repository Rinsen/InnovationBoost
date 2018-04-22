using System;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public class ExternalApplication
    {
        public int Id { get; set; }

        public Guid ExternalApplicationId { get; set; }

        public string ApplicationKey { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset ActiveUntil { get; set; }

    }
}
