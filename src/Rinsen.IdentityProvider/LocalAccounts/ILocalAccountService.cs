using System;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.LocalAccounts
{
    public interface ILocalAccountService
    {
        Task<CreateLocalAccountResult> CreateAsync(Guid identityId, string loginId, string password);
        Task<LocalAccount> GetLocalAccountAsync(string loginId, string password);
        Task ChangePasswordAsync(string oldPassword, string newPassword);
        Task DeleteLocalAccountAsync(string password);
        Task ValidatePasswordAsync(string password);
    }
}
