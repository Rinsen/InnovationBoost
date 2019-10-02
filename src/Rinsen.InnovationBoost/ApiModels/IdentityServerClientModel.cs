using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.InnovationBoost.ApiModels
{
    public class IdentityServerClientModel : IdentityServerClient
    {

        public List<IdentityServerApiResourceModel> IdentityServerApiResources { get; set; }

        public List<IdentityServerIdentityResourceModel> IdentityServerIdentityResources { get; set; }

    }
}
