using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rinsen.IdentityProvider;

namespace Rinsen.InnovationBoost.Models
{
    public class IdentityOverview
    {
        public List<SessionModel> Sessions { get; internal set; }
    }
}
