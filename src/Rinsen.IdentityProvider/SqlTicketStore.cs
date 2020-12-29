using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Rinsen.IdentityProvider
{
    public class SqlTicketStore : ITicketStore
    {
        private readonly TicketSerializer _ticketSerializer = new TicketSerializer();
        private readonly ISessionStorage _sessionStorage;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SqlTicketStore(ISessionStorage sessionStorage,
            IHttpContextAccessor httpContextAccessor)
        {
            _sessionStorage = sessionStorage;
            _httpContextAccessor = httpContextAccessor;
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
            session.Updated = DateTimeOffset.Now;
            session.Expires = ticket.Properties.ExpiresUtc ?? DateTimeOffset.Now.AddDays(1);

            await _sessionStorage.UpdateAsync(session);
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var session = await _sessionStorage.GetAsync(key);

            if (session == default(Session))
            {
                return default;
            }

            var ticket = _ticketSerializer.Deserialize(session.SerializedTicket);

            return ticket;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var session = new Session
            {
                SessionId = ticket.Principal.GetClaimStringValue(""/*JwtClaimTypes.SessionId*/),
                IdentityId = ticket.Principal.GetClaimGuidValue(ClaimTypes.NameIdentifier),
                CorrelationId = ticket.Principal.GetClaimGuidValue(ClaimTypes.SerialNumber),
                UserAgent = string.Empty,
                IpAddress = string.Empty,
                Created = DateTimeOffset.Now,
                Updated = DateTimeOffset.Now,
                Deleted = null,
                Expires = ticket.Properties.ExpiresUtc ?? DateTimeOffset.Now.AddDays(1),
                SerializedTicket = _ticketSerializer.Serialize(ticket)
            };

            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext != null)
            {
                var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
                if (remoteIpAddress != null)
                {
                    session.IpAddress = remoteIpAddress.ToString();
                }

                var userAgent = httpContext.Request.Headers["User-Agent"];
                if (!string.IsNullOrEmpty(userAgent))
                {
                    if (userAgent.Count > 200)
                    {
                        session.UserAgent = userAgent.ToString().Substring(0, 200);
                    }
                    else
                    {
                        session.UserAgent = userAgent;
                    }
                }
            }

            await _sessionStorage.CreateAsync(session);

            return session.SessionId;

        }
    }
}
