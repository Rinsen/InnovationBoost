using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Rinsen.IdentityProvider.LocalAccounts;

namespace Rinsen.InnovationBoost.Models
{
    public class TwoFactorModel
    {
        public string ReturnUrl { get; set; }
        public bool RequestTwoFactor { get; set; }
        public bool TwoFactorEmailEnabled { get; set; }
        public bool TwoFactorSmsEnabled { get; set; }
        public bool TwoFactorTotpEnabled { get; set; }
        public bool TwoFactorAppNotificationEnabled { get; set; }
        public TwoFactorType TypeSelected { get; set; }
        public string KeyCode { get; set; }

    }
}
