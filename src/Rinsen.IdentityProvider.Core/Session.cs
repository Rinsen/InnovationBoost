using System;
using System.Net;

namespace Rinsen.IdentityProvider.Core
{
    public class Session
    {
        public int ClusteredId { get; set; }
        public string SessionId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid IdentityId { get; set; }
        public DateTimeOffset LastAccess { get; set; }
        public DateTimeOffset Expires { get; set; }
        public byte[] SerializedTicket { get; set; }

    }
}
