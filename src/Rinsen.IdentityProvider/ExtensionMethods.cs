using Microsoft.Extensions.DependencyInjection;
using Rinsen.IdentityProvider.ExternalApplications;
using Rinsen.IdentityProvider.LocalAccounts;
using Rinsen.IdentityProvider.Core;
using System;

namespace Rinsen.IdentityProvider
{
    public static class ExtensionMethods
    {
        public static void AddRinsenIdentity(this IServiceCollection services, Action<IdentityOptions> identityOptionsAction)
        {
            var identityOptions = new IdentityOptions();

            identityOptionsAction.Invoke(identityOptions);

            services.AddSingleton(identityOptions);

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<PasswordHashGenerator, PasswordHashGenerator>();
            services.AddScoped<ILocalAccountService, LocalAccountService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IExternalApplicationService, ExternalApplicationService>();
            services.AddScoped<ILocalAccountStorage, LocalAccountStorage>();
            services.AddScoped<IIdentityStorage, IdentityStorage>();
            services.AddScoped<ISessionStorage, SessionStorage>();
            services.AddScoped<IExternalApplicationStorage, ExternalApplicationStorage>();
            services.AddScoped<IExternalSessionStorage, ExternalSessionStorage>();
            services.AddScoped<ITokenStorage, TokenStorage>();
            services.AddScoped<IIdentityAccessor, IdentityAccessService>();
            services.AddScoped<IIdentityAttributeStorage, IdentityAttributeStorage>();

            services.AddRinsenAuthentication();
        }
    }
}
