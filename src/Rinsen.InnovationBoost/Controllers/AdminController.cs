using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider.IdentityServer;

namespace Rinsen.InnovationBoost.Controllers
{
    public class AdminController : Controller
    {
        private readonly IdentityServerDefaultInstaller _identityServerDefaultInstaller;

        public AdminController(IdentityServerDefaultInstaller identityServerDefaultInstaller)
        {
            _identityServerDefaultInstaller = identityServerDefaultInstaller;
        }

        [AllowAnonymous]
        public async Task<IActionResult> InstallDefault()
        {
            var credentials = await _identityServerDefaultInstaller.Install();

            return Ok(credentials);
        }
    }
}
