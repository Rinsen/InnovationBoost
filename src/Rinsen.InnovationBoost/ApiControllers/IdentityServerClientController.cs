using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider.IdentityServer;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.InnovationBoost.ApiControllers
{
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
            var identityServerClient = await _identityServerClientBusiness.GetIdentityServerClients();

            return identityServerClient;
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

            return identityServerClient;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<IdentityServerClient>> Create([Required]string clientId, [Required]string clientName, [Required]string description)
        {
            await _identityServerClientBusiness.CreateNewClient(clientId, clientName, description);

            return await _identityServerClientBusiness.GetIdentityServerClient(clientId);
        }



    }
}
