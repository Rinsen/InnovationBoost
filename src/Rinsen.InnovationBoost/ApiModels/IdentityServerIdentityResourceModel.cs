using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.ApiModels
{
    public class IdentityServerIdentityResourceModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public bool Enabled { get; set; }

        public bool Checked { get; set; }
    }
}
