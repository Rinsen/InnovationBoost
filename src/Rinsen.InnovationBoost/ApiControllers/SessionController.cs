using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Rinsen.InnovationBoost.ApiControllers
{
    [Route("api/[controller]")]
    public class SessionController : Controller
    {
        private readonly ISessionStorage _sessionStorage;

        public SessionController(ISessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }


        // GET: api/<controller>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public Task<Session> Get(string id)
        {
            return _sessionStorage.GetAsync(id);
        }

        // POST api/<controller>
        [HttpPost]
        public Task Post([FromBody]Session session)
        {
            return _sessionStorage.CreateAsync(session);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public Task Put(string id, [FromBody]Session session)
        {
            return _sessionStorage.UpdateAsync(session);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public Task Delete(string id)
        {
            return _sessionStorage.DeleteAsync(id);
        }
    }
}
