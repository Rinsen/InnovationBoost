using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.Models
{
    public class ExternalApplicationToCreate
    {
        public string Name { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset ActiveUntil { get; set; }

    }
}
