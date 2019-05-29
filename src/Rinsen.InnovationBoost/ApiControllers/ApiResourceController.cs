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
    public class ApiResourceController : Controller
    {
        private readonly IdentityServerApiResourceBusiness _identityServerApiResourceBusiness;

        public ApiResourceController(IdentityServerApiResourceBusiness identityServerApiResourceBusiness)
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
            var identityServerApiResource = await _identityServerApiResourceBusiness.GetIdentityServerApiResourceAsync(id);

            if (identityServerApiResource == default(IdentityServerApiResource))
                return NotFound();

            return identityServerApiResource;
        }

        [HttpDelete("{id}", Name = "DeleteApiResource")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(string id)
        {
            await _identityServerApiResourceBusiness.DeleteIdentityServerApiResourceAsync(id);

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/Create")]
        public async Task<ActionResult<IdentityServerApiResource>> Create([Required]CreateApiResource createApiResource)
        {
            await _identityServerApiResourceBusiness.CreateNewApiResource(createApiResource.Name, createApiResource.DisplayName, createApiResource.Description);

            var identityServerApiResource = await _identityServerApiResourceBusiness.GetIdentityServerApiResourceAsync(createApiResource.Name);

            return CreatedAtAction(nameof(GetById),
               new { id = createApiResource.Name }, identityServerApiResource);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/Update")]
        public async Task<ActionResult> Update([Required]IdentityServerApiResource identityServerApiResource)
        {
            await _identityServerApiResourceBusiness.UpdateApiResource(identityServerApiResource);

            return Ok();
        }
    }
}
