using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackClientFamily : ICreatedAndUpdatedTimestamp, ISoftDelete
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Deleted { get; set; }

        [JsonIgnore]
        public virtual List<OutbackClient> Clients { get; set; }

    }
}
