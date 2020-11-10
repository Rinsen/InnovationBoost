using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OpenIdConnectSample
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
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Events.OnTokenResponseReceived = async (context) => {
                    //var bodyAsText = await new System.IO.StreamReader(context.Response.Body).ReadToEndAsync();
                    var source = new CancellationTokenSource();
                    var configuration = await context.Options.ConfigurationManager.GetConfigurationAsync(source.Token);
                };

                options.Events.OnTokenValidated = (context) => {
                    //var bodyAsText = await new System.IO.StreamReader(context.Response.Body).ReadToEndAsync();

                    return Task.CompletedTask;
                };

                options.SignInScheme = "Cookies";
                options.ClientId = "6e074b24-1f7a-4f9e-96e3-45c9d517499c";
                options.ClientSecret = "zL_O3hTErk1oz4kvn9DWBEUuQPdG9moaGYROIZmpEAI";
                //options.Authority = "https://innovationboost.azurewebsites.net/";
                options.Authority = "https://localhost:44391/";
                //options.Authority = "https://localhost:44350/";
                options.ResponseType = "code";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseMiddleware<TestMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(routes =>
            {
                routes.MapDefaultControllerRoute();
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

            if (context.Request.Path == "/signin-oidc")
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

