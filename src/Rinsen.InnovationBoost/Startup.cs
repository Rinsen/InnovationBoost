using System;
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
using Rinsen.IdentityProvider.IdentityServer;
using Rinsen.InnovationBoost.Installation.IdentityServer;
using Rinsen.InnovationBoost.Installation;
using Rinsen.Messaging;
using Microsoft.Extensions.Hosting;
using IdentityServer4.Configuration;
using IdentityServer4;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["Rinsen:ConnectionString"];
            var httpContextAccessor = new HttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor>(httpContextAccessor);

            if (_env.IsDevelopment())
            {
                // Register the Swagger generator, defining 1 or more Swagger documents
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                    // Swagger 2.+ support
                    //var security = new Dictionary<string, IEnumerable<string>>
                    //{
                    //    {"Bearer", new string[] { }},
                    //};

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                            },
                            new[] { "readAccess", "writeAccess" }
                        }
                    });
                });
            }
            services.AddRinsenIdentity(options => options.ConnectionString = connectionString);

            ConfigureIdentityServer(services);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminsOnly", policy => policy.RequireClaim("http://rinsen.se/Administrator"));
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.SessionStore = new SqlTicketStore(new SessionStorage(connectionString), httpContextAccessor);
                    options.LoginPath = "/Identity/Login";
                    options.LogoutPath = "/Identity/LogOut";

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
                options.UseSqlServer(connectionString));

            services.AddDbContext<IdentityServerDbContext>(options =>
            options.UseSqlServer(connectionString));

            var builder = services.AddMvc(o =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                o.Filters.Add(new AuthorizeFilter(policy));

            })
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);

#if DEBUG
            if (_env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            
            if (_env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();

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

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseIdentityServer();
            
            app.UseEndpoints(routes =>
            {
                routes.MapDefaultControllerRoute();
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
                .AddDeviceFlowStore<IdentityServerDeviceFlowStore>()
                .AddProfileService<IdentityServerProfileService>();
        }

        private void AddSigningCredentials(IIdentityServerBuilder identityServerBuilder)
        {
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(Configuration["Rinsen:RsaKeyFile"]);
                var rsaKeyFile = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                var rsaKey = JsonConvert.DeserializeObject<RsaKey>(rsaKeyFile, new JsonSerializerSettings { ContractResolver = new RsaKeyContractResolver() });

                identityServerBuilder.AddSigningCredential(CryptoHelper.CreateRsaSecurityKey(rsaKey.Parameters, rsaKey.KeyId), IdentityServerConstants.RsaSigningAlgorithm.RS256);
            }
            catch (Exception)
            {
                
            }
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


