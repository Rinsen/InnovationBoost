using System;

namespace Rinsen.IdentityProvider
{
    public class TokenAlreadyExistException : Exception
    {
        public TokenAlreadyExistException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
