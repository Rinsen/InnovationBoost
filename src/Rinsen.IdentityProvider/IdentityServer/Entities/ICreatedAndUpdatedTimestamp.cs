using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public interface ICreatedAndUpdatedTimestamp
    {
        DateTimeOffset Created { get; set; }

        DateTimeOffset Updated { get; set; }
    }
}
