using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.Models
{
    public class DeviceConcentModel
    {
        public string RememberConcent { get; set; }

        public IEnumerable<string> ScopeConcented { get; set; }

        public string AcceptButton { get; set; }

        public string UserCode { get; set; }

    }
}
