using System;

namespace Rinsen.IdentityProvider
{
    public class ReferenceIdentity
    {
        public int ClusteredId { get; set; }
        public Guid IdentityId { get; set; }
        public DateTimeOffset Created { get; set; }

    }
}
