using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rinsen.Logger;
using Rinsen.Logger.Service;

namespace Rinsen.InnovationBoostApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggerController : Controller
    {
        private readonly LogHandler _logHandler;
        private readonly ILogApplicationHandler _logApplicationHandler;

        public LoggerController(LogHandler logHandler,
            ILogApplicationHandler logApplicationHandler)
        {
            _logHandler = logHandler;
            _logApplicationHandler = logApplicationHandler;
        }

        [HttpPost]
        public async Task Create([FromBody]IEnumerable<LogItem> logItems)
        {
            var logItemsList = logItems.ToList();

            var logApplicationId = await _logApplicationHandler.GetLogApplicationIdAsync("");

            await _logHandler.CreateLogs(logItemsList, logApplicationId);

        }

    }
}
