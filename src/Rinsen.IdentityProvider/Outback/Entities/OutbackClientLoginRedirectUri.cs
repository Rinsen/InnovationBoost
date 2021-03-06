﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Outback.Entities
{
    public class OutbackClientLoginRedirectUri : ICreatedAndUpdatedTimestamp, ISoftDelete
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string LoginRedirectUri { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Deleted { get; set; }


        [JsonIgnore]
        public virtual OutbackClient Client { get; set; }
    }
}
