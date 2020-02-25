﻿using System;
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

        public bool RequestTwoFactor { get; set; }
        public bool TwoFactorEmailEnabled { get; set; }
        public bool TwoFactorSmsEnabled { get; set; }
        public bool TwoFactorTotpEnabled { get; set; }
        public bool TwoFactorAppNotificationEnabled { get; set; }

    }
}
