using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rinsen.OAuth.Controllers
{
    [ApiController]
    [Route("connect")]
    public class ConnectController : Controller
    {

        [HttpGet]
        [Route("authorize")]
        public IActionResult Authorize([FromQuery]AuthorizeModel model)
        {
            if (ModelState.IsValid)
            {
                // Get client
                var client = new Client { 
                    ClientId = ""
                };

                // Validare scopes and return url

                // Collect consent if needed

                // Generate and store grant

                // Return code 

                return View(new AuthorizeResponse
                {
                    Code = "hejhopp",
                    FormPostUri = model.RedirectUri,
                    Scope = model.Scope,
                    SessionState = "hejhoppihgen",
                    State = model.State
                });
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("token")]
        public IActionResult Token([FromForm] TokenModel model)
        {
            if (ModelState.IsValid)
            {
                // Get client

                // Validate client secret if needed

                // Validate return url if provided



                
            }

            return BadRequest(ModelState);
        }
        // EndSession

        // userinfo

        // checksession

        // revocation

        // introspect

        // deviceauthorization









    }

    public class Client
    {
        public string ClientId { get; set; }

    }

    public class TokenModel : IValidatableObject
    {

        [Required]
        [BindProperty(Name = "grant_type")]
        public string GrantType { get; set; }

        [Required]
        [BindProperty(Name = "code")]
        public string Code { get; set; }

        [BindProperty(Name = "redirect_uri")]
        public string RedirectUri { get; set; }

        [Required]
        [BindProperty(Name = "client_id")]
        public string ClientId { get; set; }

        [BindProperty(Name = "client_secret")]
        public string ClientSecret { get; set; }

        [Required]
        [BindProperty(Name = "code_verifier")]
        public string CodeVerifier { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResult = new List<ValidationResult>();

            if (GrantType != "authorization_code")
            {
                validationResult.Add(new ValidationResult($"grant_type must be set to authorization_code", new[] { nameof(GrantType) }));
            }

            return validationResult;
        }
    }

    public class AuthorizeModel : IValidatableObject
    {
        [Required]
        [FromQuery(Name = "client_id")]
        public string ClientId { get; set; }

        [Required]
        [FromQuery(Name = "code_challenge")]
        public string CodeChallenge { get; set; }
        
        [Required]
        [FromQuery(Name = "code_challenge_method")]
        public string CodeChallengeMethod { get; set; }

        [FromQuery(Name = "nonce")]
        public string Nonce { get; set; }

        [FromQuery(Name = "redirect_uri")]
        public string RedirectUri { get; set; }

        [Required]
        [FromQuery(Name = "response_type")]
        public string ResponseType { get; set; }

        [FromQuery(Name = "scope")]
        public string Scope { get; set; }

        [FromQuery(Name = "state")]
        public string State { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResult = new List<ValidationResult>();

            if (ResponseType != "code")
            {
                validationResult.Add(new ValidationResult($"response_type {ResponseType} is not supported", new[] { nameof(ResponseType) }));
            }

            if (CodeChallengeMethod != "S256")
            {
                validationResult.Add(new ValidationResult($"code_challenge_method {CodeChallengeMethod} is not supported", new[] { nameof(CodeChallengeMethod) }));
            }

            return validationResult;
        }
    }

    public class AuthorizeResponse
    {
        [HiddenInput]
        public string Code { get; set; }

        [HiddenInput]
        public string Scope { get; set; }

        [HiddenInput]
        public string State { get; set; }

        [HiddenInput]
        public string SessionState { get; set; }

        public string FormPostUri { get; set; }

    }
}
