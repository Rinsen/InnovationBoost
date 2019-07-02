using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider.Core;

namespace Rinsen.InnovationBoost.ApiControllers
{
    [ApiController]
    [Route("IdentityServer/api/[controller]")]
    [Authorize("AdminsOnly")]
    public class RandomController : Controller
    {
        private readonly RandomStringGenerator _randomStringGenerator;

        public RandomController(RandomStringGenerator randomStringGenerator)
        {
            _randomStringGenerator = randomStringGenerator;
        }

        [HttpGet("{count}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public ActionResult<string> Get(int count)
        {
            return _randomStringGenerator.GetRandomString(count);
        }
    }
}
