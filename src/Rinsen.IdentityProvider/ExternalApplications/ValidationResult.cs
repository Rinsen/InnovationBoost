using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public class ValidationResult
    {
        public bool Succeeded { get; private set; }
        public bool Failed { get { return !Succeeded; } }
        public string Token { get; private set; }

        public static ValidationResult Failure()
        {
            return new ValidationResult() { Succeeded = false };
        }

        public static ValidationResult Success(string token)
        {
            return new ValidationResult() { Succeeded = true, Token = token };
        }
    }
}
