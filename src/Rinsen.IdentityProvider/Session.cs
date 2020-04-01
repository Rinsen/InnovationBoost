using System;

namespace Rinsen.IdentityProvider
{
    public class Session
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid IdentityId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public DateTimeOffset Expires { get; set; }
        public byte[] SerializedTicket { get; set; }

    }
}
