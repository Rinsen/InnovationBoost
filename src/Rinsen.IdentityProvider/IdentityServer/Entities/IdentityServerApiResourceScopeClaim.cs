using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerApiResourceScopeClaim
    {
        public int Id { get; set; }

        public int ApiResourceScopeId { get; set; }

        public string Type { get; set; }


    }
}
