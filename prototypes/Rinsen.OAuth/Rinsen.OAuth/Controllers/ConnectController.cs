using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rinsen.OAuth.Controllers
{
    [ApiController]
    [Route("connect")]
    public class ConnectController : Controller
    {
        private readonly EllipticCurveJsonWebKeyService _ellipticCurveJsonWebKeyService;

        public ConnectController(EllipticCurveJsonWebKeyService ellipticCurveJsonWebKeyService)
        {
            _ellipticCurveJsonWebKeyService = ellipticCurveJsonWebKeyService;
        }

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
                //var rsa = RSA.Create(2048);

                
                var myIssuer = "http://mysite.com";
                var myAudience = "http://myaudience.com";

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, "userId1234"),
                        }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Issuer = myIssuer,
                    IssuedAt = DateTime.Now,
                    Audience = myAudience,
                    SigningCredentials = new SigningCredentials(_ellipticCurveJsonWebKeyService.GetJsonWebKeyForSigning(), SecurityAlgorithms.EcdsaSha256)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                Response.Headers.Add("Cache-Control", "no-store");
                Response.Headers.Add("Pragma", "no-cache");

                return Json(new TokenResponse
                {
                    AccessToken = tokenString,
                    ExpiresIn = 3600,
                    TokenType = "Bearer"
                });
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

    public class EllipticCurveJsonWebKeyService
    {
        private readonly JsonWebKey _jwk;
        private readonly EllipticCurveJsonWebKeyModel _ellipticCurveJsonWebKeyModel;

        public EllipticCurveJsonWebKeyService()
        {
            var secret = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var key = new ECDsaSecurityKey(secret);

            _jwk = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(key);
            _ellipticCurveJsonWebKeyModel = new EllipticCurveJsonWebKeyModel
            {
                KeyId = _jwk.KeyId,
                X = Base64UrlEncoder.Encode(_jwk.X),
                Y = Base64UrlEncoder.Encode(_jwk.Y),
            };
        } 

        public JsonWebKey GetJsonWebKeyForSigning()
        {
            return _jwk;
        }

        public EllipticCurveJsonWebKeyModel GetEllipticCurveJsonWebKeyModel()
        {
            return _ellipticCurveJsonWebKeyModel;
        }

    }

    public class EllipticCurveJsonWebKeyModel
    {
        [JsonPropertyName("kty")]
        public string KeyType { get { return "EC"; } }

        [JsonPropertyName("use")]
        public string PublicKeyUse { get { return "sig"; } }

        [JsonPropertyName("kid")]
        public string KeyId { get; set; }

        [JsonPropertyName("x")]
        public string X { get; set; }

        [JsonPropertyName("y")]
        public string Y { get; set; }

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

    public class RefreshTokenResponse : TokenResponse
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
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
        public string Code { get; set; }

        public string Scope { get; set; }

        public string State { get; set; }

        public string SessionState { get; set; }

        public string FormPostUri { get; set; }

    }
}
