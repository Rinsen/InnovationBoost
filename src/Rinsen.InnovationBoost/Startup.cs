using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rinsen.DatabaseInstaller;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.Core;
using Rinsen.InnovationBoost.Installation;
using Rinsen.Messaging;

namespace Rinsen.InnovationBoost
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsDevelopment())
            {
                services.AddDatabaseInstaller(Configuration["Rinsen:ConnectionString"]);
            }
            services.AddRinsenIdentity(options => options.ConnectionString = Configuration["Rinsen:ConnectionString"]);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.SessionStore = new SqlTicketStore(new SessionStorage(Configuration["Rinsen:ConnectionString"]));
                    options.LoginPath = "/Identity/Login";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminsOnly", policy => policy.RequireClaim("http://rinsen.se/Administrator"));
            });

            services.Remove(new ServiceDescriptor(typeof(ILoginService), typeof(LoginService), ServiceLifetime.Scoped));

            services.AddScoped<ILoginService, IdentityWebLoginService>();

            services.AddRinsenMessaging();

            services.AddDbContext<MessageDbContext>(options =>
                options.UseSqlServer(Configuration["Rinsen:ConnectionString"]));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)//, ILogger<Startup> logger)
        {
            if (_env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();

                app.UseDatabaseInstaller(options =>
                {
                    options.DatabaseVersions.Add(new CreateTables());
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>

            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //logger.LogInformation("Starting");
        }
    }
}
