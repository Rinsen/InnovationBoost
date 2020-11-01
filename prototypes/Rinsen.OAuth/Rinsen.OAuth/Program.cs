using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Rinsen.OAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var secret = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var key = new ECDsaSecurityKey(secret);

            var jwk = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(key);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
