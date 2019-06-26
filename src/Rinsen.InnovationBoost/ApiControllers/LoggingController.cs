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

            var logApplication = await _logApplicationHandler.GetLogApplicationAsync(User.Claims.First(m => m.Type == "client_id").Value);

            await _logHandler.CreateLogs(logItemsList, logApplication.Id);
        }

    }
}
