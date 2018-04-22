using System;
using System.Threading.Tasks;
using Rinsen.IdentityProvider.Contracts.v1;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public interface IExternalApplicationService
    {
        Task<ValidationResult> GetTokenForValidHostAsync(string applicationName, string host, Guid identityId, Guid correlationId, bool rememberMe);
        Task<Token> GetTokenAsync(string tokenId, string applicationKey);
        Task LogExportedExternalIdentity(ExternalIdentity externalIdentity, Guid externalApplicationId);
    }
}
