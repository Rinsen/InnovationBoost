using System;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public class Token
    {
        public int ClusteredId { get; set; }
        public string TokenId { get; set; }
        public Guid ExternalApplicationId { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid IdentityId { get; set; }
        public bool Expiration { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
