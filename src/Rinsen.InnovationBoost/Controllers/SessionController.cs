using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider;

namespace Rinsen.InnovationBoost.Controllers
{
    public class SessionController : Controller
    {
        private readonly IIdentityAccessor _identityAccessor;

        public SessionController(IIdentityAccessor identityAccessor)
        {
            _identityAccessor = identityAccessor;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            


            return new string[] { "value1", "value2" };
        }

    }
}
