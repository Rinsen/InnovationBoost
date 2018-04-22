using System;
using System.Security.Claims;

namespace Rinsen.IdentityProvider.Core
{
    public interface IIdentityAccessor
    {
        ClaimsPrincipal ClaimsPrincipal { get; }
        Guid IdentityId { get; }
    }
}
