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
    public class IdentityServerIdentityResourceController : Controller
    {
        private readonly IdentityServerIdentityResourceBusiness _identityServerIdentityResourceBusiness;

        public IdentityServerIdentityResourceController(IdentityServerIdentityResourceBusiness identityServerIdentityResourceBusiness)
        {
            _identityServerIdentityResourceBusiness = identityServerIdentityResourceBusiness;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<List<IdentityServerIdentityResource>>> GetAll()
        {
            var identityServerApiResources = await _identityServerIdentityResourceBusiness.GetIdentityServerIdentityResourcesAsync();

            return identityServerApiResources;
        }

        [HttpGet("{id}", Name = "GetApiResource")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IdentityServerIdentityResource>> GetById(string id)
        {
            var identityServerIdentityResource = await _identityServerIdentityResourceBusiness.GetIdentityServerIdentityResourceAsync(id);

            if (identityServerIdentityResource == default(IdentityServerIdentityResource))
                return NotFound();

            return identityServerIdentityResource;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/api/[controller]/Create")]
        public async Task<ActionResult<IdentityServerIdentityResource>> Create([Required]string name, [Required]string displayName, [Required]string description)
        {
            await _identityServerIdentityResourceBusiness.CreateNewIdentityResourceAsync(name, displayName, description);

            var identityServerApiResource = await _identityServerIdentityResourceBusiness.GetIdentityServerIdentityResourceAsync(name);

            return CreatedAtAction(nameof(GetById),
               new { id = name }, identityServerApiResource);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/api/[controller]/Update")]
        public async Task<ActionResult> Update([Required]IdentityServerIdentityResource identityServerIdentityResource)
        {
            await _identityServerIdentityResourceBusiness.UpdateIdentityResourceAsync(identityServerIdentityResource);

            return Ok();
        }
    }
}
