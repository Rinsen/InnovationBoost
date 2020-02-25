using System;

namespace Rinsen.IdentityProvider
{
    public class SessionAlreadyExistException : Exception
    {
        public SessionAlreadyExistException(string message, Exception innerException)
            :base(message, innerException)
        {

        }

    }
}
