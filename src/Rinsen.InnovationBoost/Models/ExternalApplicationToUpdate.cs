using System;

namespace Rinsen.InnovationBoost.Models
{
    public class ExternalApplicationToUpdate
    {
        public Guid ExternalApplicationId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset ActiveUntil { get; set; }
    }
}
