using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackClientSecret : ICreatedAndUpdatedTimestamp, ISoftDelete
    {

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Deleted { get; set; }
    }
}
