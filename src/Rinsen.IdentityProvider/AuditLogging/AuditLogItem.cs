using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.AuditLogging
{
    public class AuditLogItem
    {
        public int Id { get; set; }

        public string EventType { get; set; }

        public string Details { get; set; }

        public string IpAddress { get; set; }

        public DateTimeOffset Timestamp { get; set; }

    }
}
