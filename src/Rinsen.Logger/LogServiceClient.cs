using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public class LogServiceClient : ILogServiceClient
    {
        private LogOptions _options;
        private readonly IConfiguration _configuration;
        private AccessTokenResult _accessTokenResult;

        public LogServiceClient(LogOptions options,
            IConfiguration configuration)
        {
            _options = options;
            _configuration = configuration;
        }

        public async Task ReportAsync(IEnumerable<LogItem> logItems)
        {
            string accessToken;
            try
            {
                accessToken = await GetAccessToken();
            }
            catch (Exception e)
            {
                throw new AccessTokenException("Failed to get access token", e);
            }

            using (var httpClient = new HttpClient())
            {
#if DEBUG
                httpClient.Timeout = TimeSpan.FromMinutes(10);
#endif
                httpClient.SetBearerToken(accessToken);

                var serializedLogs = JsonSerializer.Serialize(logItems);
                var stringContent = new StringContent(serializedLogs, Encoding.UTF8, "application/json");
                using (var result = await httpClient.PostAsync($"{_options.LogServiceUri}api/Logging", stringContent))
                {
                    result.EnsureSuccessStatusCode();
                }
            }
        }

        private async Task<string> GetAccessToken()
        {
            if (_accessTokenResult == null || _accessTokenResult.Expires < DateTime.Now)
            {
                using (var protocolClient = new HttpClient())
                {
                    var disco = await protocolClient.GetDiscoveryDocumentAsync(_options.LogServiceUri);

                    if (disco.IsError)
                        throw new Exception(disco.Error);

                    var tokenResponse = await protocolClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = _configuration["Rinsen:ClientId"],
                        ClientSecret = _configuration["Rinsen:ClientSecret"]
                    });

                    if (tokenResponse.IsError)
                    {
                        throw new Exception(tokenResponse.Error);
                    }

                    _accessTokenResult = new AccessTokenResult
                    {
                        AccessToken = tokenResponse.AccessToken,
                        Expires = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn)
                    };
                }
            }

            return _accessTokenResult.AccessToken;
        }

        private class AccessTokenResult
        {
            public DateTime Expires  { get; set; }

            public string AccessToken { get; set; }

        }
    }

    public class AccessTokenException : Exception
    {
        public AccessTokenException(string message, Exception innerException)
            :base(message, innerException)
        {

        }
    }
}
