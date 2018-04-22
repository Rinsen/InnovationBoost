using System;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public class ExternalApplicationHostName
    {
        public Guid ExternalApplicationId { get; set; }

        public string Hostname { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset ActiveUntil { get; set; }
    }
}
