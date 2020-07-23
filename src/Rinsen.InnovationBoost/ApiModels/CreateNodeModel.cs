using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.ApiModels
{
    public class CreateNodeModel
    {
        [Required]
        public string ClientName { get; set; }

        [Required]
        public string ClientDescription { get; set; }

    }
}
