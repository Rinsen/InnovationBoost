using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerClientRedirectUri
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string RedirectUri { get; set; }
    }
}
