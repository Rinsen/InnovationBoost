using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerApiResource
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public bool Enabled { get; set; }

        public string Name { get; set; }

        public List<IdentityServerApiResourceSecret> ApiSecrets { get; set; }
        
        public List<IdentityServerApiResourceScope> Scopes { get; set; }

        public List<IdentityServerApiResourceClaim> Claims { get; set; }

    }
}
