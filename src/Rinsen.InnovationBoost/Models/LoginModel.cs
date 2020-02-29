using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public bool InvalidEmailOrPassword { get; set; }
        public string ReturnUrl { get; set; }

    }
}
