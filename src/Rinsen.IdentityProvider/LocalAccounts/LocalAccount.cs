using System;

namespace Rinsen.IdentityProvider.LocalAccounts
{
    public class LocalAccount
    {
        public int Id { get; set; }

        public Guid IdentityId { get; set; }

        public string LoginId { get; set; }
        
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public int IterationCount { get; set; }

        public int FailedLoginCount { get; set; }

        public bool IsDisabled { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }
    }
}
