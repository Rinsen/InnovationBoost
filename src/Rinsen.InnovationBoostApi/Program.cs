using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rinsen.Logger.Service;

namespace Rinsen.InnovationBoostApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddEnvironmentVariables();
                    if (env.IsDevelopment())
                    {
                        config.AddUserSecrets<Startup>();
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                    logging
                        .AddFilter("Microsoft", LogLevel.Warning)
                        //.AddFilter("Microsoft.EntityFrameworkCore.Database.Command")
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("Rinsen", LogLevel.Information)
                        .AddLoggerService(hostingContext.Configuration, hostingContext.HostingEnvironment.EnvironmentName);
                })
                .UseStartup<Startup>()
                .Build();

            webHost.Run();
        }
    }
}
