using System.ComponentModel.DataAnnotations;
using Rinsen.Outback.Clients;

namespace Rinsen.InnovationBoost.Models
{
    public class CreateFamily
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
