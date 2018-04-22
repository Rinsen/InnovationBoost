using System;

namespace Rinsen.IdentityProvider
{
    public class ExternalApplicationAlreadyExistException : Exception
    {
        public ExternalApplicationAlreadyExistException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
