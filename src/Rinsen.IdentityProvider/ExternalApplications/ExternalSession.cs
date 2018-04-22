using System;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public class ExternalSession
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid IdentityId { get; set; }
        public Guid ExternalApplicationId { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
