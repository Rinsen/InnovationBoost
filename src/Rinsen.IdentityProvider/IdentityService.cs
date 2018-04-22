using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rinsen.IdentityProvider.LocalAccounts;
using System;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider
{
    public class IdentityService : IIdentityService
    {
        readonly IIdentityAccessor _claimsPrincipalAccessor;
        readonly IdentityOptions _identityOptions;
        readonly IIdentityStorage _identityStorage;
        readonly ILocalAccountService _localAccountService;
        readonly ILogger _log;
        readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityService(IIdentityAccessor claimsPrincipalAccessor,
            IdentityOptions identityOptions,
            IIdentityStorage identityStorage,
            ILocalAccountService localAccountService,
            ILogger<IdentityService> log,
            IHttpContextAccessor httpContextAccessor)
        {
            _claimsPrincipalAccessor = claimsPrincipalAccessor;
            _identityOptions = identityOptions;
            _identityStorage = identityStorage;
            _localAccountService = localAccountService;
            _httpContextAccessor = httpContextAccessor;
            _log = log;
        }

        public async Task<CreateIdentityResult> CreateAsync(string firstName, string lastName, string email, string phoneNumber)
        {
            var identity = new Identity
            {
                IdentityId = Guid.NewGuid(),
                Created = DateTimeOffset.Now,
                Email = email,
                EmailConfirmed = false,
                GivenName = firstName,
                Surname = lastName,
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = false,
                Updated = DateTimeOffset.Now
            };

            try
            {
                await _identityStorage.CreateAsync(identity);
            }
            catch (IdentityAlreadyExistException e)
            {
                _log.LogWarning(0, e, $"Identity {identity.Email} already exist from address {_httpContextAccessor.HttpContext.Connection.RemoteIpAddress}");
                return CreateIdentityResult.AlreadyExist();
            }

            _log.LogInformation($"New identity created for email {identity.Email}, with name {identity.GivenName}, {identity.Surname} and phone number {identity.PhoneNumber}");

            return CreateIdentityResult.Success(identity);
        }
        
        public async Task<Identity> GetIdentityAsync()
        {
            return await _identityStorage.GetAsync(_claimsPrincipalAccessor.IdentityId);
        }

        public async Task<Identity> GetIdentityAsync(Guid identityId)
        {
            return await _identityStorage.GetAsync(identityId);
        }

        public void UpdateIdentityDetails(string firstName, string lastName, string email, string phoneNumber)
        {
            throw new NotImplementedException();
        }
    }
}
