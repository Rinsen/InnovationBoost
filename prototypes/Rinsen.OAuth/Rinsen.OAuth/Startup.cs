using System.Security.Claims;
using System.Threading.Tasks;
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
            var secretStorage = new SecretStorage(new RandomStringGenerator());
            services.AddSingleton<ITokenSigningAccessor>(secretStorage);
            services.AddSingleton<IWellKnownSigningAccessor>(secretStorage);
            services.AddSingleton<IGrantAccessor, GrantStorage>();
            services.AddSingleton<IClientAccessor, ClientStorage>();

            services.AddRinsenOutback();

            services.AddMvc()
                .AddApplicationPart(typeof(Client).Assembly);
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

            app.UseMiddleware<FakeUserMiddleware>();

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

    public class FakeUserMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public FakeUserMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                            new Claim("sub", "Kalle123")
                }));

            // Call the next delegate/middleware in the pipeline
            await _requestDelegate(context);


        }

    }
}
