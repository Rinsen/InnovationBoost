using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rinsen.Logger
{
    public class RinsenRequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RinsenRequestLogMiddleware> _logger;
        private readonly Stopwatch _stopwatch;
        const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed} ms for identity {identityId}";

        public RinsenRequestLogMiddleware(RequestDelegate next,
            ILogger<RinsenRequestLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if(_logger.IsEnabled(LogLevel.Debug))
            {
                _stopwatch.Start();

                try
                {
                    LogRequest(httpContext);
                }
                catch (Exception)
                {
                    LogRequest(httpContext);

                    throw;
                }
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }

        private void LogRequest(HttpContext httpContext)
        {
            _stopwatch.Stop();
            var elapsedMs = _stopwatch.ElapsedMilliseconds;
            _stopwatch.Reset();

            var statusCode = httpContext.Response?.StatusCode;

            if (httpContext.User.Identity.IsAuthenticated)
            {
                var identityId = httpContext.User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).Single().Value;

                _logger.LogInformation(MessageTemplate, httpContext.Request.Method, GetPath(httpContext), statusCode, elapsedMs, identityId);

                return;
            }

            _logger.LogInformation(MessageTemplate, httpContext.Request.Method, GetPath(httpContext), statusCode, elapsedMs, "unknown");
            
        }

        private static string GetPath(HttpContext httpContext)
        {
            return httpContext.Features.Get<IHttpRequestFeature>()?.RawTarget ?? httpContext.Request.Path.ToString();
        }
    }
}
