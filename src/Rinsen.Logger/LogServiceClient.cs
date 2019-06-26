using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public class LogServiceClient : ILogServiceClient
    {
        private LogOptions _options;
        private readonly IConfiguration _configuration;

        public LogServiceClient(LogOptions options,
            IConfiguration configuration)
        {
            _options = options;
            _configuration = configuration;
        }

        public async Task<bool> ReportAsync(LogReport logReport)
        {
            TokenResponse tokenResponse;
            using (var protocolClient = new HttpClient())
            {
                var disco = await protocolClient.GetDiscoveryDocumentAsync(_options.LogServiceUri);
                if (disco.IsError) throw new Exception(disco.Error);

                tokenResponse = await protocolClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = _configuration["Rinsen:ClientId"],
                    ClientSecret = _configuration["Rinsen:ClientSecret"]
                });
            }

            using (var httpClient = new HttpClient())
            {
#if DEBUG
                httpClient.Timeout = TimeSpan.FromMinutes(10);
#endif
                httpClient.SetBearerToken(tokenResponse.AccessToken);

                var serializedLogs = JsonConvert.SerializeObject(logReport);
                var stringContent = new StringContent(serializedLogs, Encoding.UTF8, "application/json");
                using (var result = await httpClient.PostAsync($"{_options.LogServiceUri}api/Logging", stringContent))
                {
                    result.EnsureSuccessStatusCode();
                    var result2 = await result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(result2);
                }
            }
        }
    }
}
