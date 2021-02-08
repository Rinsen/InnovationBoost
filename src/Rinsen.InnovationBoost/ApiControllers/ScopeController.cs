using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.InnovationBoost.ApiModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Rinsen.InnovationBoost.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScopeController : ControllerBase
    {
        // GET: api/<ScopeController>
        [HttpGet]
        public IEnumerable<OutbackScope> Get()
        {
            return new List<OutbackScope>();
        }

        // GET api/<ScopeController>/5
        [HttpGet("{id}")]
        public OutbackScope Get(int id)
        {
            return new OutbackScope();
        }

        // POST api/<ScopeController>
        [HttpPost]
        public void Post([FromBody]CreateScopeModel value)
        {
        }

        // PUT api/<ScopeController>/5
        [HttpPut]
        public void Put([FromBody] OutbackScope value)
        {
        }

        // DELETE api/<ScopeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
