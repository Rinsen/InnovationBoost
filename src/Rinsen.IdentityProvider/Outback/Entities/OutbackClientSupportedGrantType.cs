using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackClientSupportedGrantType : ICreatedAndUpdatedTimestamp, ISoftDelete
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string GrantType { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Deleted { get; set; }
    }
}
