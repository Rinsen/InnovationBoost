    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.Models
{
    public class CreateClientType
    {

        [Required]
        public string DisplayName { get; set; }

    }
}
