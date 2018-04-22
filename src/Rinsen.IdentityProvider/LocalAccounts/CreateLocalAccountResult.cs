using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.LocalAccounts
{
    public class CreateLocalAccountResult
    {
        public CreateLocalAccountResult()
        {
        }

        public CreateLocalAccountResult(LocalAccount localAccount)
        {
            LocalAccount = localAccount;
        }

        public bool LocalAccountAlreadyExist { get; private set; }
        public bool Succeeded { get; private set; }
        public LocalAccount LocalAccount { get; }

        public static CreateLocalAccountResult AlreadyExist()
        {
            return new CreateLocalAccountResult() { LocalAccountAlreadyExist = true };
        }

        public static CreateLocalAccountResult Success(LocalAccount localAccount)
        {
            return new CreateLocalAccountResult(localAccount) { Succeeded = true };
        }
    }
}
