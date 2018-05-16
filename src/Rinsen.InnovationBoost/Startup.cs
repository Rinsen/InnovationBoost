using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rinsen.DatabaseInstaller;
using Rinsen.Logger.Service;
using Rinsen.Logger.Service.Installation;

namespace Rinsen.InnovationBoost
{
    public class Startup
    {

        // https://github.com/aspnet/LibraryManager

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services, IHostingEnvironment env)
        {
            services.AddLoggerService(options =>
            {
                options.ConnectionString = Configuration["Rinsen:ConnectionString"];
            });

            if (env.IsDevelopment())
            {
                services.AddDatabaseInstaller(Configuration["Rinsen:ConnectionString"]);
            }

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();

                app.RunDatabaseInstaller(new List<DatabaseVersion>
                {
                    new CreateTables()
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
        }
    }
}
