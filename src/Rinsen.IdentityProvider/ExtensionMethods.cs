using Microsoft.Extensions.DependencyInjection;
using Rinsen.IdentityProvider.LocalAccounts;
using Rinsen.IdentityProvider.Core;
using System;
using Rinsen.IdentityProvider.IdentityServer;
using Rinsen.IdentityProvider.AuditLogging;

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
            services.AddScoped<ILocalAccountStorage, LocalAccountStorage>();
            services.AddScoped<IIdentityStorage, IdentityStorage>();
            //services.AddScoped<ISessionStorage, SessionStorage>();
            services.AddScoped<IIdentityAccessor, IdentityAccessService>();
            services.AddScoped<IIdentityAttributeStorage, IdentityAttributeStorage>();
            services.AddTransient<IdentityServerClientBusiness, IdentityServerClientBusiness>();
            services.AddTransient<IdentityServerApiResourceBusiness, IdentityServerApiResourceBusiness>();
            services.AddTransient<IdentityServerIdentityResourceBusiness, IdentityServerIdentityResourceBusiness>();
            services.AddScoped<AuditLog, AuditLog>();
            services.AddScoped<AuditLogStorage, AuditLogStorage>();

            services.AddRinsenAuthentication();
        }
    }
}
