using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.Models
{
    public class SessionModel
    {
        public int Id { get; set; }
        public string ClientDescrition { get; internal set; }
        public DateTimeOffset Created { get; internal set; }
        public DateTimeOffset Expires { get; internal set; }
        public string IpAddress { get; internal set; }
        public bool CurrentSession { get; internal set; }
    }
}
