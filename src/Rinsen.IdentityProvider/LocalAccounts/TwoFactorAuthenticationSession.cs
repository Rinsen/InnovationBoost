using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.LocalAccounts
{
    public class TwoFactorAuthenticationSession
    {
        public int Id { get; set; }

        public Guid IdentityId { get; set; }

        public string SessionId { get; set; }

        public TwoFactorType Type { get; set; }

        public string KeyCode { get; set; }

        public DateTimeOffset Created { get; set; }

    }

    public enum TwoFactorType
    {
        NotSelected,
        Totp,
        Sms,
        Email,
        Notification
    }
}


