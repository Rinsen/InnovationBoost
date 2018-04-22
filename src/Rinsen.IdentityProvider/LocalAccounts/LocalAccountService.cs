using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.LocalAccounts
{
    public class LocalAccountService : ILocalAccountService
    {
        private readonly IIdentityAccessor _identityAccessor;
        private readonly ILocalAccountStorage _localAccountStorage;
        private readonly IdentityOptions _options;
        private readonly PasswordHashGenerator _passwordHashGenerator;
        private readonly ILogger<LocalAccountService> _log;

        private static readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();

        public LocalAccountService(IIdentityAccessor identityAccessor,
            ILocalAccountStorage localAccountStorage,
            IdentityOptions options,
            PasswordHashGenerator passwordHashGenerator,
            ILogger<LocalAccountService> log)
        {
            _localAccountStorage = localAccountStorage;
            _identityAccessor = identityAccessor;
            _options = options;
            _passwordHashGenerator = passwordHashGenerator;
            _identityAccessor = identityAccessor;
            _log = log;
        }


        public async Task ChangePasswordAsync(string oldPassword, string newPassword)
        {
            var localAccount = await GetLocalAccountAsync(oldPassword);

            localAccount.PasswordHash = GetPasswordHash(newPassword, localAccount);
            localAccount.Updated = DateTimeOffset.Now;

            await _localAccountStorage.UpdateAsync(localAccount);
        }

        public async Task<CreateLocalAccountResult> CreateAsync(Guid identityId, string loginId, string password)
        {
            var bytes = new byte[_options.NumberOfBytesInPasswordSalt];
            CryptoRandom.GetBytes(bytes);

            var localAccount = new LocalAccount
            {
                IdentityId = identityId,
                IterationCount = _options.IterationCount,
                PasswordSalt = bytes,
                LoginId = loginId,
                Created = DateTimeOffset.Now,
                Updated = DateTimeOffset.Now
            };

            localAccount.PasswordHash = GetPasswordHash(password, localAccount);

            try
            {
                await _localAccountStorage.CreateAsync(localAccount);
            }
            catch (IdentityAlreadyExistException e)
            {
                _log.LogError(0, e, "Local account already exist");

                return CreateLocalAccountResult.AlreadyExist();
            }

            return CreateLocalAccountResult.Success(localAccount);
        }

        private byte[] GetPasswordHash(string password, LocalAccount localAccount)
        {
            return _passwordHashGenerator.GetPasswordHash(localAccount.PasswordSalt, password, localAccount.IterationCount, _options.NumberOfBytesInPasswordHash);
        }

        public async Task DeleteLocalAccountAsync(string password)
        {
            var localAccount = await GetLocalAccountAsync(password);

            await _localAccountStorage.DeleteAsync(localAccount);
        }

        private async Task<LocalAccount> GetLocalAccountAsync(string password)
        {
            var localAccount = await _localAccountStorage.GetAsync(_identityAccessor.IdentityId);

            ValidatePassword(localAccount, password);

            return localAccount;
        }
        
        public async Task<Guid?> GetIdentityIdAsync(string loginId, string password)
        {
            var localAccount = await _localAccountStorage.GetAsync(loginId);

            if (localAccount == default(LocalAccount))
            {
                return null;
            }

            ValidatePassword(localAccount, password);
            
            if (localAccount.FailedLoginCount > 0)
            {
                SetFailedLoginCountToZero(localAccount);
            }

            return localAccount.IdentityId;
        }

        private void SetFailedLoginCountToZero(LocalAccount localAccount)
        {
            localAccount.FailedLoginCount = 0;
            localAccount.Updated = DateTimeOffset.Now;
            _localAccountStorage.UpdateFailedLoginCountAsync(localAccount);
        }

        public async Task ValidatePasswordAsync(string password)
        {
            var localAccount = await _localAccountStorage.GetAsync(_identityAccessor.IdentityId);
            ValidatePassword(localAccount, password);
        }

        private void ValidatePassword(LocalAccount localAccount, string password)
        {
            if (!localAccount.PasswordHash.SequenceEqual(GetPasswordHash(password, localAccount)))
            {
                InvalidPassword(localAccount);
            }
        }

        private void InvalidPassword(LocalAccount localAccount)
        {
            localAccount.FailedLoginCount++;
            localAccount.Updated = DateTimeOffset.Now;

            _log.LogWarning("Invalid password for local account {0} with iteration count {1}", localAccount.IdentityId, localAccount.IterationCount);

            _localAccountStorage.UpdateFailedLoginCountAsync(localAccount);

            throw new UnauthorizedAccessException("Invalid password");
        }
    }
}
