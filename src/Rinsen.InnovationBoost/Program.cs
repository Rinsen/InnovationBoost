using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using Rinsen.Logger.Service;
using Microsoft.Extensions.Hosting;

namespace Rinsen.InnovationBoost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        var env = hostingContext.HostingEnvironment;
                        if (env.IsDevelopment())
                        {
                            logging.AddFilter("Microsoft", LogLevel.Warning)
                                    .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information)
                                    .AddFilter("IdentityServer4", LogLevel.Information)
                                    .AddFilter("System", LogLevel.Warning)
                                    .AddFilter("Rinsen", LogLevel.Information).AddConsole()
                                    .AddLoggerService(hostingContext.Configuration, hostingContext.HostingEnvironment.EnvironmentName);

                        }
                        else
                        {
                            logging.AddFilter("Microsoft", LogLevel.Warning)
                                    .AddFilter("IdentityServer4", LogLevel.Warning)
                                    .AddFilter("System", LogLevel.Warning)
                                    .AddFilter("Rinsen", LogLevel.Information)
                                    .AddLoggerService(hostingContext.Configuration, hostingContext.HostingEnvironment.EnvironmentName);
                        }
                    });
                });
    }
}
