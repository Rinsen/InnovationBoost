using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerApiResourceProperty : ICreatedAndUpdatedTimestamp, IObjectWithState
    {
        public int Id { get; set; }

        public int ApiResourceId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public ObjectState State { get; set; }

    }
}
