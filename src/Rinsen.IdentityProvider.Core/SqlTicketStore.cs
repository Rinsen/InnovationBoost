using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Rinsen.IdentityProvider.Core
{
    public class SqlTicketStore : ITicketStore
    {
        private readonly TicketSerializer _ticketSerializer = new TicketSerializer();
        private readonly ISessionStorage _sessionStorage;
        private readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();

        public SqlTicketStore(ISessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public async Task RemoveAsync(string key)
        {
            await _sessionStorage.DeleteAsync(key);
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var session = await _sessionStorage.GetAsync(key);

            var serializedTicket = _ticketSerializer.Serialize(ticket);

            session.SerializedTicket = serializedTicket;
            session.LastAccess = DateTimeOffset.Now;
            session.Expires = ticket.Properties.ExpiresUtc ?? DateTimeOffset.Now.AddDays(1);

            await _sessionStorage.UpdateAsync(session);
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var session = await _sessionStorage.GetAsync(key);

            if (session == default(Session))
            {
                return default(AuthenticationTicket);
            }

            var ticket = _ticketSerializer.Deserialize(session.SerializedTicket);

            return ticket;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var bytes = new byte[32];
            CryptoRandom.GetBytes(bytes);
            var sessionId = Base64UrlTextEncoder.Encode(bytes);

            var session = new Session
            {
                SessionId = sessionId,
                IdentityId = ticket.Principal.GetClaimGuidValue(ClaimTypes.NameIdentifier),
                CorrelationId = ticket.Principal.GetClaimGuidValue(ClaimTypes.SerialNumber),
                LastAccess = DateTimeOffset.Now,
                Expires = ticket.Properties.ExpiresUtc ?? DateTimeOffset.Now.AddDays(1),
                SerializedTicket = _ticketSerializer.Serialize(ticket)
            };

            await _sessionStorage.CreateAsync(session);

            return session.SessionId;
        }
    }
}