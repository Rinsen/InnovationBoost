using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerClientRedirectUri : ICreatedAndUpdatedTimestamp, IObjectWithState
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string RedirectUri { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public ObjectState State { get; set; }
    }
}
