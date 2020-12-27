﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rinsen.OAuth.InMemoryStorage;
using Rinsen.Outback;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Clients;
using Rinsen.Outback.Grants;

namespace Rinsen.OAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<RandomStringGenerator>();
            services.AddScoped<GrantService>();
            services.AddScoped<ClientService>();
            services.AddScoped<TokenFactory>();

            var secretStorage = new SecretStorage(new RandomStringGenerator());
            services.AddSingleton<ITokenSigningStorage>(secretStorage);
            services.AddSingleton<IWellKnownSigningStorage>(secretStorage);
            services.AddSingleton<IGrantStorage, GrantStorage>();
            services.AddSingleton<IClientStorage, ClientStorage>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMiddleware<TestMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class TestMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public TestMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            if (context.Request.Path == "/connect/token")
            {
                //  Enable seeking
                context.Request.EnableBuffering();
                //  Read the stream as text
                var bodyAsText = await new System.IO.StreamReader(context.Request.Body).ReadToEndAsync();
                //  Set the position of the stream to 0 to enable rereading
                context.Request.Body.Position = 0;
            }
            // Call the next delegate/middleware in the pipeline
            await _requestDelegate(context);


        }

    }
}
