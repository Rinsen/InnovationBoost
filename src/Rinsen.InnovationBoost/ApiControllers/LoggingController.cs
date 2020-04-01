using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinsen.Logger;
using Rinsen.Logger.Service;

namespace Rinsen.InnovationBoost.ApiControllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class LoggingController : Controller
    {
        private readonly LogHandler _logHandler;
        private readonly ILogApplicationHandler _logApplicationHandler;

        public LoggingController(LogHandler logHandler,
            ILogApplicationHandler logApplicationHandler)
        {
            _logHandler = logHandler;
            _logApplicationHandler = logApplicationHandler;
        }

        [HttpPost]
        public async Task Create([FromBody]IEnumerable<LogItem> logItems)
        {
            var logItemsList = logItems.ToList();

            var applicationId = User.Claims.First(m => m.Type == "client_id").Value;
            var displayName = GetDisplayName();

            var logApplication = await _logApplicationHandler.GetLogApplicationAsync(applicationId, displayName);

            await _logHandler.CreateLogs(logItemsList, logApplication.Id);
        }

        private string GetDisplayName()
        {
            var nameClaim = User.Claims.FirstOrDefault(m => m.Type == "client_name");

            if (nameClaim != default)
            {
                return nameClaim.Value;
            }

            nameClaim = User.Claims.FirstOrDefault(m => m.Type == "client_nodename");

            if (nameClaim != default)
            {
                return nameClaim.Value;
            }

            return string.Empty;
        }
    }
}
