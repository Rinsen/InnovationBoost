using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerApiResourceSecret
    {
        public int Id { get; set; }

        public int ApiResourceId { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Expiration { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

    }
}
