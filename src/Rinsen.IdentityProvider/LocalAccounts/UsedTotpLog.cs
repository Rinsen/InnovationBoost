using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.LocalAccounts
{
    public class UsedTotpLog
    {
        public int Id { get; set; }

        public Guid IdentityId { get; set; }

        public string Code { get; set; }

        public DateTimeOffset UsedTime { get; set; }

    }
}
