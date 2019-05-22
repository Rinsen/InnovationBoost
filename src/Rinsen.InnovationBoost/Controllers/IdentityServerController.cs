using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Rinsen.InnovationBoost.Controllers
{
    public class IdentityServerController : Controller
    {
        public IActionResult Clients()
        {
            return View();
        }

        public IActionResult ApiResources()
        {
            return View();
        }

        public IActionResult IdentityResources()
        {
            return View();
        }
    }
}
