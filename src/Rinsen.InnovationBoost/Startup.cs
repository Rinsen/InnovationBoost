﻿using System;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rinsen.DatabaseInstaller;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.Core;
using Rinsen.IdentityProvider.IdentityServer;
using Rinsen.InnovationBoost.Installation.IdentityServer;
using Rinsen.InnovationBoost.Installation;
using Rinsen.Messaging;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;

namespace Rinsen.InnovationBoost
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _env = env;
            _loggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsDevelopment())
            {
                services.AddDatabaseInstaller(Configuration["Rinsen:ConnectionString"]);

                //// Register the Swagger generator, defining 1 or more Swagger documents
                //services.AddSwaggerGen(c =>
                //{
                //    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                //});

                // Register the Swagger generator, defining 1 or more Swagger documents
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

                    // Swagger 2.+ support
                    var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                    c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                    });
                    c.AddSecurityRequirement(security);
                });
            }
            services.AddRinsenIdentity(options => options.ConnectionString = Configuration["Rinsen:ConnectionString"]);

            ConfigureIdentityServer(services);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminsOnly", policy => policy.RequireClaim("http://rinsen.se/Administrator"));
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.SessionStore = new SqlTicketStore(new SessionStorage(Configuration["Rinsen:ConnectionString"]));
                    options.LoginPath = "/Identity/Login";

                    options.ForwardDefaultSelector = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api"))
                        {
                            return "Bearer";
                        }
                        else
                        {
                            return "Cookies";
                        }
                    };
                })
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["Rinsen:InnovationBoost"];
                    options.Audience = Configuration["Rinsen:Audience"];

                    options.ForwardDefaultSelector = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api"))
                        {
                            return "Bearer";
                        }
                        else
                        {
                            return "Cookies";
                        }
                    };
                });
            
            services.AddRinsenMessaging();

            services.AddDbContext<MessageDbContext>(options =>
                options.UseSqlServer(Configuration["Rinsen:ConnectionString"]));

            services.AddDbContext<IdentityServerDbContext>(options =>
                options.UseLoggerFactory(_loggerFactory)
                .UseSqlServer(Configuration["Rinsen:ConnectionString"]));

            services.AddMvc(o =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                o.Filters.Add(new AuthorizeFilter(policy));

            } ).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            var a = new IdentityServerTableInstallation();
            var script = a.UpCommands;

            if (_env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();

                app.UseDatabaseInstaller(options =>
                {
                    options.DatabaseVersions.Add(new CreateTables());
                    options.DatabaseVersions.Add(new CreateSettingsTable());
                    options.DatabaseVersions.Add(new IdentityServerTableInstallation());
                    options.DatabaseVersions.Add(new IdentityServerDeviceFlowCodesAdded());
                    options.DatabaseVersions.Add(new CreateLogTables());
                });

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });

            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            logger.LogInformation("Starting");
        }

        private void ConfigureIdentityServer(IServiceCollection services)
        {
            var identityServerBuilder = services.AddIdentityServer();

            if (_env.IsDevelopment())
            {
                identityServerBuilder.AddDeveloperSigningCredential();
            }
            else
            {
                AddSigningCredentials(identityServerBuilder);
            }

            identityServerBuilder
                .AddClientStore<IdentityServiceClientStore>()
                .AddResourceStore<IdentityServerResourceStore>()
                .AddPersistedGrantStore<IdentityServerPersistedGrantStore>()
                .AddDeviceFlowStore<IdentityServerDeviceFlowStore>();
        }

        private void AddSigningCredentials(IIdentityServerBuilder identityServerBuilder)
        {
            var base64EncodedBytes = Convert.FromBase64String(Configuration["Rinsen:RsaKeyFile"]);
            var rsaKeyFile = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            var rsaKey = JsonConvert.DeserializeObject<RsaKey>(rsaKeyFile, new JsonSerializerSettings { ContractResolver = new RsaKeyContractResolver() });

            identityServerBuilder.AddSigningCredential(IdentityServerBuilderExtensionsCrypto.CreateRsaSecurityKey(rsaKey.Parameters, rsaKey.KeyId));
        }

        private class RsaKey
        {
            public string KeyId { get; set; }
            public RSAParameters Parameters { get; set; }
        }

        private class RsaKeyContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                property.Ignored = false;

                return property;
            }
        }
    }
}

