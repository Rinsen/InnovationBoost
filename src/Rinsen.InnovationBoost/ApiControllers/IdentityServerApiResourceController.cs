using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider.IdentityServer;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.InnovationBoost.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize("AdminsOnly")]
    public class IdentityServerApiResourceController : Controller
    {
        private readonly IdentityServerApiResourceBusiness _identityServerApiResourceBusiness;

        public IdentityServerApiResourceController(IdentityServerApiResourceBusiness identityServerApiResourceBusiness)
        {
            _identityServerApiResourceBusiness = identityServerApiResourceBusiness;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<List<IdentityServerApiResource>>> GetAll()
        {
            var identityServerApiResources = await _identityServerApiResourceBusiness.GetIdentityServerApiResources();

            return identityServerApiResources;
        }

        [HttpGet("{id}", Name = "GetApiResource")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IdentityServerApiResource>> GetById(string id)
        {
            var identityServerApiResource = await _identityServerApiResourceBusiness.GetIdentityServerApiResource(id);

            if (identityServerApiResource == default(IdentityServerApiResource))
                return NotFound();

            return identityServerApiResource;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/api/[controller]/Create")]
        public async Task<ActionResult<IdentityServerApiResource>> Create([Required]string name, [Required]string displayName, [Required]string description)
        {
            await _identityServerApiResourceBusiness.CreateNewApiResource(name, displayName, description);

            var identityServerApiResource = await _identityServerApiResourceBusiness.GetIdentityServerApiResource(name);

            return CreatedAtAction(nameof(GetById),
               new { id = name }, identityServerApiResource);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/api/[controller]/Update")]
        public async Task<ActionResult> Update([Required]IdentityServerApiResource identityServerApiResource)
        {
            await _identityServerApiResourceBusiness.UpdateApiResource(identityServerApiResource);

            return Ok();
        }
    }
}
