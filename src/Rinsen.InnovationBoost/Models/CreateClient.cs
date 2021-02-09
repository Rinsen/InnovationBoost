using System.ComponentModel.DataAnnotations;
using Rinsen.Outback.Clients;

namespace Rinsen.InnovationBoost.Models
{
    public class CreateClient
    {
        [Required]
        public string ClientName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public ClientType ClientType { get; set; }

        [Required]
        public int FamilyId { get; set; }
    }
}
