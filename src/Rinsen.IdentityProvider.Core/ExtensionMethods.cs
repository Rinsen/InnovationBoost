using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Security.Claims;

namespace Rinsen.IdentityProvider.Core
{
    public static class ExtensionMethods
    {
        public static string GetClaimStringValue(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            return claimsPrincipal.GetClaimStringValue(m => m.Type == claimType);
        }

        public static string GetClaimStringValue(this ClaimsPrincipal claimsPrincipal, Predicate<Claim> match)
        {
            if (claimsPrincipal.HasClaim(match))
            {
                try
                {
                    return claimsPrincipal.Claims.Where(new Func<Claim, bool>(match)).Single().Value;
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("The claims collection does not contain exactly one element.");
                }

            }
            else
            {
                throw new InvalidOperationException("The claims collection does not contain a element that match.");
            }
        }

        public static int GetClaimIntValue(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            return claimsPrincipal.GetClaimIntValue(m => m.Type == claimType);
        }

        public static int GetClaimIntValue(this ClaimsPrincipal claimsPrincipal, Predicate<Claim> match)
        {
            int result;
            if (!int.TryParse(claimsPrincipal.GetClaimStringValue(match), out result))
            {
                throw new InvalidOperationException("Parse exception in claims value");
            }

            return result;
        }

        public static Guid GetClaimGuidValue(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            return claimsPrincipal.GetClaimGuidValue(m => m.Type == claimType);
        }

        public static Guid GetClaimGuidValue(this ClaimsPrincipal claimsPrincipal, Predicate<Claim> match)
        {
            Guid result;
            if (!Guid.TryParse(claimsPrincipal.GetClaimStringValue(match), out result))
            {
                throw new InvalidOperationException("Parse exception in claims value");
            }

            return result;
        }

        public static bool GetClaimBoolValue(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            return claimsPrincipal.GetClaimBoolValue(m => m.Type == claimType);
        }

        public static bool GetClaimBoolValue(this ClaimsPrincipal claimsPrincipal, Predicate<Claim> match)
        {
            if (claimsPrincipal.HasClaim(match))
            {
                try
                {
                    return claimsPrincipal.Claims.Where(new Func<Claim, bool>(match)).Single().Value.Equals("True", StringComparison.InvariantCultureIgnoreCase);
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidOperationException("The claims collection does not contain exactly one element.");
                }
            }
            else
            {
                throw new InvalidOperationException("The claims collection does not contain a element that match.");
            }
        }

        public static void AddRinsenAuthentication(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IIdentityAccessor, IdentityAccessService>();
        }
    }
}
