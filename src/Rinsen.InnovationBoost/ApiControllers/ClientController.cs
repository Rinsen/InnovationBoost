using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider.Outback;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.InnovationBoost.Models;

namespace Rinsen.InnovationBoost.ApiControllers
{
    [ApiController]
    [Route("Outback/api/[controller]")]
    [Authorize("AdminsOnly")]
    public class ClientController : Controller
    {
        private readonly ClientService _clientService;

        public ClientController(ClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<List<OutbackClient>>> GetAll()
        {
            var clients = await _clientService.GetAll();

            clients.ForEach(c => c.Secrets.ForEach(s => s.Secret = "****"));

            return clients;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OutbackClient>> GetById(string id)
        {
            var client = await _clientService.GetClient(id);

            if (client == default)
                return NotFound();

            client.Secrets.ForEach(s => s.Secret = "****");

            return client;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(string id)
        {
            await _clientService.DeleteIdentityServerClient(id);

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<OutbackClient>> Create([Required] CreateClient createClient)
        {
            var clientId = Guid.NewGuid().ToString();

            await _clientService.CreateNewClient(clientId, createClient.ClientName, createClient.Description, createClient.FamilyId, createClient.ClientType);

            var client = await _clientService.GetClient(clientId);

            return CreatedAtAction(nameof(GetById),
               new { id = clientId }, client);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<ActionResult> Update(int id, [FromBody][Required] OutbackClient client)
        {
            await _clientService.UpdateClient(id, client);

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [Route("~/Outback/api/[controller]/Family")]
        public async Task<ActionResult<List<OutbackClientFamily>>> GetAllClientFamilies()
        {
            var clientFamilies = await _clientService.GetAllClientFamilies();

            return clientFamilies;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Route("~/Outback/api/[controller]/Family")]
        public async Task<ActionResult<OutbackClientFamily>> CreateFamily([Required] CreateFamily createFamily)
        {
            var family = await _clientService.CreateNewFamily(createFamily.Name, createFamily.Description);

            return CreatedAtAction(nameof(GetById),
               new { id = family.Id }, family);
        }
    }
}
