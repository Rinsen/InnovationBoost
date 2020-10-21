using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rinsen.OAuth.Controllers
{
    [ApiController]
    [Route("connect")]
    public class ConnectController : Controller
    {

        // Authorize
        [HttpGet]
        [Route("authorize")]
        public string Authorize([FromQuery]AuthorizeModel model)
        {
            throw new NotImplementedException();
        }
        // Token

        // EndSession

        // userinfo

        // checksession

        // revocation

        // introspect

        // deviceauthorization









    }

    public class AuthorizeModel
    {

        [FromQuery(Name = "client_id")]
        public string ClientId { get; set; }

        [FromQuery(Name = "code_challenge")]
        public string CodeChallenge { get; set; }
        
        [FromQuery(Name = "code_challenge_method")]
        public string CodeChallengeMethod { get; set; }

        [FromQuery(Name = "nonce")]
        public string Nonce { get; set; }

        [FromQuery(Name = "redirect_uri")]
        public string RedirectUri { get; set; }
        
        [FromQuery(Name = "response_type")]
        public string ResponseType { get; set; }

        [FromQuery(Name = "state")]
        public string State { get; set; }
            
    }
}
