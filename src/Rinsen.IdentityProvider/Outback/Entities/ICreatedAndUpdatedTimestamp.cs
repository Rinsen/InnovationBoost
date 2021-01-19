using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public interface ICreatedAndUpdatedTimestamp
    {
        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

    }
}
