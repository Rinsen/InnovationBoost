using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.ApiModels
{
    public class IdentityServerApiResourceModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public bool Enabled { get; set; }

        public bool Selected { get; set; }

        public List<IdentityServerApiResourceScopeModel> IdentityServerApiResourceScopes { get; set; } = new List<IdentityServerApiResourceScopeModel>();
    }

    public class IdentityServerApiResourceScopeModel
    {

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Selected { get; set; }
    }
}
