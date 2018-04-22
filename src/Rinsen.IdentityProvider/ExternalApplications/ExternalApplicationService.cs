using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Rinsen.IdentityProvider.Contracts.v1;
using System;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.ExternalApplications
{
    public class ExternalApplicationService : IExternalApplicationService
    {
        private readonly IExternalApplicationStorage _externalApplicationStorage;
        private readonly IExternalSessionStorage _externalSessionStorage;
        private readonly ITokenStorage _tokenStorage;
        private readonly ILogger<ExternalApplicationService> _log;

        private static readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();

        public ExternalApplicationService(IExternalApplicationStorage externalApplicationStorage,
            IExternalSessionStorage externalSessionStorage,
            ITokenStorage tokenStorage,
            ILogger<ExternalApplicationService> log)
        {
            _externalApplicationStorage = externalApplicationStorage;
            _externalSessionStorage = externalSessionStorage;
            _tokenStorage = tokenStorage;
            _log = log;
        }

        public async Task<Token> GetTokenAsync(string tokenId, string applicationKey)
        {
            if (string.IsNullOrEmpty(tokenId))
            {
                throw new ArgumentException("TokenId is required", nameof(tokenId));
            }

            if (string.IsNullOrEmpty(applicationKey))
            {
                throw new ArgumentException("Application key is required", nameof(applicationKey));
            }

            var token = await _tokenStorage.GetAndDeleteAsync(tokenId);
            var externalApplication = await _externalApplicationStorage.GetFromApplicationKeyAsync(applicationKey);

            if (externalApplication.Active 
                && externalApplication.ExternalApplicationId == token.ExternalApplicationId 
                && token.Created.AddSeconds(15) >= DateTimeOffset.Now)
            {


                return token;
            }

            throw new AuthenticationException($"Authentication failed for token id {tokenId} and application key {applicationKey}");
        }

        public async Task<ValidationResult> GetTokenForValidHostAsync(string applicationName, string host, Guid identityId, Guid correlationId, bool rememberMe)
        {
            if (string.IsNullOrEmpty(host))
                return ValidationResult.Failure();

            var externalApplication = await _externalApplicationStorage.GetFromApplicationNameAndHostAsync(applicationName, host);

            if (externalApplication == default(ExternalApplication))
            {
                return  ValidationResult.Failure();
            }

            var bytes = new byte[32];
            CryptoRandom.GetBytes(bytes);
            var tokenId = Base64UrlTextEncoder.Encode(bytes);

            var token = new Token
            {
                TokenId = tokenId,
                Created = DateTimeOffset.Now,
                ExternalApplicationId = externalApplication.ExternalApplicationId,
                IdentityId = identityId,
                Expiration = rememberMe,
                CorrelationId = correlationId
            };

            await _tokenStorage.CreateAsync(token);

            return ValidationResult.Success(token.TokenId);
        }

        public Task LogExportedExternalIdentity(ExternalIdentity externalIdentity, Guid externalApplicationId)
        {
            var externalSession = new ExternalSession
            {
                CorrelationId = externalIdentity.CorrelationId,
                Created = DateTimeOffset.Now,
                ExternalApplicationId = externalApplicationId,
                IdentityId = externalIdentity.IdentityId
            };

            return _externalSessionStorage.Create(externalSession);
        }
    }
}
