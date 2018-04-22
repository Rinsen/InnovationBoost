using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Rinsen.IdentityProvider.Contracts;
using Microsoft.Extensions.Configuration;
using Rinsen.IdentityProvider.Core;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider.Token
{
    public static class TokenExtensions
    {
        /// <summary>
        /// Add default configuration of Rinsen.IdentityProvider.Token with cookie support and SqlTicketStore for session storage
        /// <para/>Connection string for storing session data and local user for reference ConnectionString
        /// <para/>IdentityProvider:ApplicationKey
        /// <para/>IdentityProvider:IdentityServiceUrl
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddDefaultTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = GetConnectionString(configuration);

            var localIdentityForReferenceHandler = new LocalIdentityForReferenceHandler(connectionString);

            return services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                    .AddToken(TokenDefaults.AuthenticationScheme, options =>
                    {

                        options.CallbackPath = new PathString("/token");
                        options.ClaimsIssuer = RinsenIdentityConstants.RinsenIdentityProvider;
                        options.ApplicationName = configuration["Rinsen:ApplicationName"];
                        options.ApplicationKey = configuration["Rinsen:ApplicationKey"];
                        options.IdentityServiceUrl = configuration["Rinsen:IdentityProvider:BaseUrl"];
                        options.Events = new RemoteAuthenticationEvents
                        {
                            OnTicketReceived = async context =>
                            {
                                await localIdentityForReferenceHandler.CreateReferenceIdentityIfNotExisting(context.Principal);
                            }
                        };
                    })
                    .AddCookie(options =>
                    {
                        options.SessionStore = new SqlTicketStore(new SessionStorage(connectionString));
                        options.Events.OnRedirectToLogin = ctx =>
                        {
                            ctx.HttpContext.ChallengeAsync(TokenDefaults.AuthenticationScheme);

                            return Task.CompletedTask;
                        };
                    });
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration["Rinsen:ConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException($"No ConnectionString provided in {nameof(configuration)}");
            }

            return connectionString;
        }

        public static AuthenticationBuilder AddToken(this AuthenticationBuilder builder, string authenticationScheme, Action<TokenOptions> configureOptions)
            => builder.AddToken(authenticationScheme, authenticationScheme, configureOptions);


        public static AuthenticationBuilder AddToken(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TokenOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TokenOptions>, TokenPostConfigureOptions>());
            return builder.AddRemoteScheme<TokenOptions, RemoteTokenHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}

