using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rinsen.Logger;

namespace LoginAndLogging
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
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConsole()
                        .AddFilter("Microsoft", LogLevel.Debug)
                        .AddFilter("System", LogLevel.Debug)
                        .AddFilter("LoggerSample", LogLevel.Debug)
                        .AddRinsenLogger(hostingContext.Configuration, hostingContext.HostingEnvironment.EnvironmentName);
                })
                .UseStartup<Startup>()
                .Build();

            webHost.Run();
        }
    }
}
