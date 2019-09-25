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
    public class ClientController : Controller
    {
        private readonly IdentityServerClientBusiness _identityServerClientBusiness;

        public ClientController(IdentityServerClientBusiness identityServerClientBusiness)
        {
            _identityServerClientBusiness = identityServerClientBusiness;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<List<IdentityServerClient>>> GetAll()
        {
            var identityServerClients = await _identityServerClientBusiness.GetIdentityServerClients();

            identityServerClients.ForEach(c => c.ClientSecrets.ForEach(s => s.Value = "****"));

            return identityServerClients;
        }

        [HttpGet("{id}", Name = "GetClient")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IdentityServerClient>> GetById(string id)
        {
            var identityServerClient = await _identityServerClientBusiness.GetIdentityServerClient(id);

            if (identityServerClient == default(IdentityServerClient))
                return NotFound();

            identityServerClient.ClientSecrets.ForEach(s => s.Value = "****");

            return identityServerClient;
        }

        [HttpDelete("{id}", Name = "DeleteClient")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(string id)
        {
            await _identityServerClientBusiness.DeleteIdentityServerClient(id);

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Route("~/IdentityServer/api/[controller]/Type")]
        public async Task<ActionResult<List<IdentityServerClientType>>> GetAllClientTypes()
        {
            var identityServerClientTypes = await _identityServerClientBusiness.GetAllClientTypes();

            return identityServerClientTypes;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/Create")]
        public async Task<ActionResult<IdentityServerClient>> Create([Required]CreateClient createClient)
        {
            await _identityServerClientBusiness.CreateNewClient(createClient.ClientId, createClient.ClientName, createClient.Description);

            var client = await _identityServerClientBusiness.GetIdentityServerClient(createClient.ClientId);

            return CreatedAtAction(nameof(GetById),
               new { id = createClient.ClientId }, client);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/CreateNode")]
        public async Task<ActionResult<IdentityServerClient>> CreateNode([Required]CreateTypedClient createClient)
        {
            var clientId = await _identityServerClientBusiness.CreateNewTypedClient(createClient.ClientName, createClient.Description, "Node");

            var client = await _identityServerClientBusiness.GetIdentityServerClient(clientId);

            return CreatedAtAction(nameof(GetById),
               new { id = clientId }, client);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/CreateWebsite")]
        public async Task<ActionResult<IdentityServerClient>> CreateWebsite([Required]CreateTypedClient createClient)
        {
            var clientId = await _identityServerClientBusiness.CreateNewTypedClient(createClient.ClientName, createClient.Description, "Website");

            var client = await _identityServerClientBusiness.GetIdentityServerClient(clientId);

            return CreatedAtAction(nameof(GetById),
               new { id = clientId }, client);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/IdentityServer/api/[controller]/Update")]
        public async Task<ActionResult> Update([Required]IdentityServerClient identityServerClient)
        {
            await _identityServerClientBusiness.UpdateClient(identityServerClient);

            return Ok();
        }



    }
}
