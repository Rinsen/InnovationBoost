using System;

namespace Rinsen.IdentityProvider
{
    public class IdentityAttribute
    {
        public int ClusteredId { get; set; }

        public Guid IdentityId { get; set; }

        public string Attribute { get; set; }
    }
}
