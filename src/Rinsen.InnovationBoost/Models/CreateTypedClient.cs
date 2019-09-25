using System.ComponentModel.DataAnnotations;

namespace Rinsen.InnovationBoost.Models
{
    public class CreateTypedClient
    {
        [Required]
        public string ClientName { get; set; }

        [Required]
        public string Description { get; set; }


    }
}
