using System;
using System.Text.Json.Serialization;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackClientClaim : ICreatedAndUpdatedTimestamp, ISoftDelete
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Deleted { get; set; }

        [JsonIgnore]
        public virtual OutbackClient Client { get; set; }
    }
}
