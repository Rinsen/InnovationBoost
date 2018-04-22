using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider
{
    public class CreateIdentityResult
    {

        public Identity Identity { get; private set; }
        public bool Succeeded { get; private set; }
        public bool IdentityAlreadyExist { get { return !Succeeded; } }

        public static CreateIdentityResult AlreadyExist()
        {
            return new CreateIdentityResult() { Succeeded = false };
        }

        public static CreateIdentityResult Success(Identity identity)
        {
            return new CreateIdentityResult() { Succeeded = true, Identity = identity };
        }
    }
}
