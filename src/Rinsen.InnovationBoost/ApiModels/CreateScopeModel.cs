using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.ApiModels
{
    public class CreateScopeModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ScopeName { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }
    }
}
