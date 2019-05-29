using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider.IdentityServer;
using Rinsen.IdentityProvider.IdentityServer.Entities;
using Rinsen.InnovationBoost.Models;

namespace Rinsen.InnovationBoost.ApiControllers
{
    [ApiController]
    [Route("IdentityServer/api/[controller]")]
    [Authorize("AdminsOnly")]
    public class IdentityResourceController : Controller
    {
        private readonly IdentityServerIdentityResourceBusiness _identityServerIdentityResourceBusiness;

        public IdentityResourceController(IdentityServerIdentityResourceBusiness identityServerIdentityResourceBusiness)
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

        [HttpGet("{id}", Name = "GetIdentityResource")]
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

        [HttpDelete("{id}", Name = "DeleteIdentityResource")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(string id)
        {
            await _identityServerIdentityResourceBusiness.DeleteIdentityServerIdentityResourceAsync(id);

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/Create")]
        public async Task<ActionResult<IdentityServerIdentityResource>> Create([Required]CreateIdentityResource createIdentityResource)
        {
            await _identityServerIdentityResourceBusiness.CreateNewIdentityResourceAsync(createIdentityResource.Name, createIdentityResource.DisplayName, createIdentityResource.Description);

            var identityServerApiResource = await _identityServerIdentityResourceBusiness.GetIdentityServerIdentityResourceAsync(createIdentityResource.Name);

            return CreatedAtAction(nameof(GetById),
               new { id = createIdentityResource.Name }, identityServerApiResource);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/Update")]
        public async Task<ActionResult> Update([Required]IdentityServerIdentityResource identityServerIdentityResource)
        {
            await _identityServerIdentityResourceBusiness.UpdateIdentityResourceAsync(identityServerIdentityResource);

            return Ok();
        }
    }
}
