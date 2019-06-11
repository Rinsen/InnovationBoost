using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerDeviceFlowCode
    {
        public int Id { get; set; }

        public string DeviceCode { get; set; }

        public string UserCode { get; set; }

        public string ClientId { get; set; }

        public Guid? IdentityId { get; set; }

        public DateTimeOffset CreationTime { get; set; }

        public DateTimeOffset? Expiration { get; set; }

        public string SerializedDeviceFlowCode { get; set; }
    }
}
