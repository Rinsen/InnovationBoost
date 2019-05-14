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
    public class IdentityServerClientController : Controller
    {
        private readonly IdentityServerClientBusiness _identityServerClientBusiness;

        public IdentityServerClientController(IdentityServerClientBusiness identityServerClientBusiness)
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

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/api/[controller]/Create")]
        public async Task<ActionResult<IdentityServerClient>> Create([Required]string clientId, [Required]string clientName, [Required]string description)
        {
            await _identityServerClientBusiness.CreateNewClient(clientId, clientName, description);

            var client = await _identityServerClientBusiness.GetIdentityServerClient(clientId);

            return CreatedAtAction(nameof(GetById),
               new { id = clientId }, client);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/api/[controller]/Update")]
        public async Task<ActionResult> Update([Required]IdentityServerClient identityServerClient)
        {
            await _identityServerClientBusiness.UpdateClient(identityServerClient);

            return Ok();
        }



    }
}
