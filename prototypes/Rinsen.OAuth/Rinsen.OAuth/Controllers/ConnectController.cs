using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Concurrent;
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
        private readonly GrantService _grantService;

        public ConnectController(EllipticCurveJsonWebKeyService ellipticCurveJsonWebKeyService,
            GrantService grantService)
        {
            _ellipticCurveJsonWebKeyService = ellipticCurveJsonWebKeyService;
            _grantService = grantService;
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

                var code = _grantService.CreateAndStoreGrant(client.ClientId, "user123", model.CodeChallenge, model.CodeChallengeMethod, model.Nonce, model.RedirectUri, model.Scope, model.State, model.ResponseType);

                // Return code 

                return View(new AuthorizeResponse
                {
                    Code = code,
                    FormPostUri = model.RedirectUri,
                    Scope = model.Scope,
                    SessionState = null,
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

                var persistedGrant = _grantService.GetGrant(model.Code);

                // Validate code

                // Validate return url if provided


                var myIssuer = "http://mysite.com";
                var myAudience = "http://myaudience.com";

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, persistedGrant.SubjectId),
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
                    IdentityToken = tokenString,
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

    public class GrantService
    {
        private readonly ConcurrentDictionary<string, PersistedGrant> _persistedGrants = new ConcurrentDictionary<string, PersistedGrant>();
        private readonly RandomStringGenerator _randomStringGenerator;

        public GrantService(RandomStringGenerator randomStringGenerator)
        {
            _randomStringGenerator = randomStringGenerator;
        }

        public PersistedGrant GetGrant(string code)
        {
            _persistedGrants.TryGetValue(code, out var persisted);

            return persisted;
        }

        internal string CreateAndStoreGrant(string clientId, string subjectId, string codeChallenge, string codeChallengeMethod, string nonce, string redirectUri, string scope, string state, string responseType)
        {
            var code = _randomStringGenerator.GetRandomString(15);
            _persistedGrants.TryAdd(code, new PersistedGrant
            {
                ClientId = clientId,
                Code = code,
                CodeChallange = codeChallenge,
                CodeChallangeMethod = codeChallengeMethod,
                Nonce = nonce,
                RedirectUri = redirectUri,
                ResponseType = responseType,
                Scope = scope,
                State = state,
                SubjectId = subjectId
            });

            return code;
        }
    }

    public class PersistedGrant
    {
        public string Code { get; set; }

        public string ClientId { get; set; }

        public string SubjectId { get; set; }

        public string CodeChallange { get; set; }

        public string CodeChallangeMethod { get; set; }

        public string Nonce { get; set; }

        public string RedirectUri { get; set; }

        public string Scope { get; set; }

        public string State { get; set; }

        public string ResponseType { get; set; }



    }

    public class EllipticCurveJsonWebKeyService
    {
        private readonly JsonWebKey _jwk;
        private readonly EllipticCurveJsonWebKeyModel _ellipticCurveJsonWebKeyModel;

        public EllipticCurveJsonWebKeyService(RandomStringGenerator randomStringGenerator)
        {
            var secret = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            var key = new ECDsaSecurityKey(secret)
            {
                KeyId = randomStringGenerator.GetRandomString(20)
            };


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

    public class RandomStringGenerator
    {
        private readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();

        public string GetRandomString(int length)
        {
            var bytes = new byte[length];

            CryptoRandom.GetBytes(bytes);

            return Base64UrlTextEncoder.Encode(bytes);
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

        [JsonPropertyName("identity_token")]
        public string IdentityToken { get; set; }

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
