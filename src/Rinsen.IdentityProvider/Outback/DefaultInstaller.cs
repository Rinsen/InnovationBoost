using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.Outback.Claims;

namespace Rinsen.IdentityProvider.Outback
{
    public class DefaultInstaller
    {
        private readonly OutbackDbContext _outbackDbContext;
        private readonly RandomStringGenerator _randomStringGenerator;

        public DefaultInstaller(OutbackDbContext outbackDbContext,
            RandomStringGenerator randomStringGenerator)
        {
            _outbackDbContext = outbackDbContext;
            _randomStringGenerator = randomStringGenerator;
        }

        public async Task<Credentials> Install()
        {
            await CreateDefaultOpenIdScopes();
            await CreateDefaultInnovationBoostScopes();
            await CreateDefaultClientFamilies();
            await CreateSigningKey();

            var creadentials = await CreateInnovationBoostWebClient();
            creadentials.SpaApplicationClientId = await CreateInnovationBoostSpaClient();

            return creadentials;
        }

        private async Task CreateSigningKey()
        {
            if (await _outbackDbContext.Secrets.AnyAsync())
            {
                return;
            }

            var secret = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            var ecdSaKey = new ECDsaSecurityKey(secret)
            {
                KeyId = _randomStringGenerator.GetRandomString(20)
            };

            var publicKey = ecdSaKey.ECDsa.ExportParameters(true);

            var cryptographyParameters = new CryptographyParameters
            {
                EncodedD = Base64UrlEncoder.Encode(publicKey.D),
                EncodedX = Base64UrlEncoder.Encode(publicKey.Q.X),
                EncodedY = Base64UrlEncoder.Encode(publicKey.Q.Y),
                KeyId = ecdSaKey.KeyId
            };

            var outbackSecret = new OutbackSecret
            {
                ActiveSigningKey = true,
                CryptographyData = JsonSerializer.Serialize(cryptographyParameters),
                Expires = DateTime.UtcNow.AddYears(100),
                PublicKeyCryptographyType = PublicKeyCryptographyType.EC_NistP256,
            };

            await _outbackDbContext.Secrets.AddAsync(outbackSecret);

            await _outbackDbContext.SaveChangesAsync();

        }

        private async Task<Credentials> CreateInnovationBoostWebClient()
        {
            if (await _outbackDbContext.Clients.AnyAsync(m => m.Name == "InnovationBoost"))
            {
                return new Credentials();
            }

            var clientFamily = await _outbackDbContext.ClientFamilies.SingleAsync(m => m.Name == "WebApplication");
            var createLogsScope = await _outbackDbContext.Scopes.SingleAsync(m => m.ScopeName == "innovationboost.createlogs");

            var secret = _randomStringGenerator.GetRandomString(30);
            var secretHashString = GetSha256Hash(secret);

            var client = new OutbackClient
            {
                ClientFamilyId = clientFamily.Id,
                ClientId = Guid.NewGuid().ToString(),
                ClientType = Rinsen.Outback.Clients.ClientType.Confidential,
                Name = "InnovationBoost",
                SupportedGrantTypes = new List<OutbackClientSupportedGrantType>
                {
                    new OutbackClientSupportedGrantType { GrantType = "client_credentials" }
                },
                Secrets = new List<OutbackClientSecret>
                {
                    new OutbackClientSecret
                    {
                        Description = "Initial secret created by installer",
                        Secret = secretHashString,
                    }
                },
                Scopes = new List<OutbackClientScope>
                {
                    new OutbackClientScope
                    {
                        ScopeId = createLogsScope.Id,
                    }
                }
            };

            await _outbackDbContext.AddAsync(client);
            await _outbackDbContext.SaveChangesAsync();

            return new Credentials
            {
                ClientId = client.ClientId,
                Secret = secret
            };
        }

        private static string GetSha256Hash(string secret)
        {
            string secretHashString = string.Empty;
            using (var mySHA256 = SHA256.Create())
            {
                var hash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(secret));

                var sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }

                secretHashString = sb.ToString();
            }

            return secretHashString;
        }

        private async Task<string> CreateInnovationBoostSpaClient()
        {
            if (await _outbackDbContext.Clients.AnyAsync(m => m.Name == "InnovationBoostAdminApp"))
            {
                return string.Empty;
            }

            var clientFamily = await _outbackDbContext.ClientFamilies.SingleAsync(m => m.Name == "SpaApplication");
            var scopes = await _outbackDbContext.Scopes.ToListAsync();

            var client = new OutbackClient
            {
                ClientFamilyId = clientFamily.Id,
                ClientType = Rinsen.Outback.Clients.ClientType.Confidential,
                ClientId = Guid.NewGuid().ToString(),
                Name = "InnovationBoostAdminApp",
                SupportedGrantTypes = new List<OutbackClientSupportedGrantType>
                {
                    new OutbackClientSupportedGrantType { GrantType = "client_credentials" }
                },
                Scopes = new List<OutbackClientScope>
                {
                    new OutbackClientScope
                    {
                        ScopeId = scopes.Single(m => m.ScopeName == "innovationboost.createlogs").Id,
                    },
                    new OutbackClientScope
                    {
                        ScopeId = scopes.Single(m => m.ScopeName == "innovationboost.administration").Id,
                    },
                    new OutbackClientScope
                    {
                        ScopeId = scopes.Single(m => m.ScopeName == "openid").Id,
                    },
                    new OutbackClientScope
                    {
                        ScopeId = scopes.Single(m => m.ScopeName == "profile").Id,
                    }
                },
                
            };

            await _outbackDbContext.AddAsync(client);
            await _outbackDbContext.SaveChangesAsync();

            return client.ClientId;
        }

        private async Task CreateDefaultInnovationBoostScopes()
        {
            var existingScopes = await _outbackDbContext.Scopes.ToListAsync();

            var scopes = new List<OutbackScope>();

            if (!existingScopes.Any(m => m.ScopeName == "innovationboost.createlogs"))
            {
                scopes.Add(new OutbackScope
                {
                    ClaimsInIdToken = false,
                    Description = "Api for creating logs",
                    DisplayName = "Log service API",
                    ScopeName = "innovationboost.createlogs",
                    ShowInDiscoveryDocument = false,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>()
                });
            }

            if (!existingScopes.Any(m => m.ScopeName == "innovationboost.administration"))
            {
                scopes.Add(new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "Api for InnovationBoost administration",
                    DisplayName = "InnovationBoost Administration",
                    ScopeName = "innovationboost.administration",
                    ShowInDiscoveryDocument = false,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>()
                });
            }

            if (!existingScopes.Any(m => m.ScopeName == "innovationboost.sessions"))
            {
                scopes.Add(new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "Api for session management",
                    DisplayName = "InnovationBoost Session management",
                    ScopeName = "innovationboost.sessions",
                    ShowInDiscoveryDocument = false,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>()
                });
            }

            if (!existingScopes.Any(m => m.ScopeName == "innovationboost.sendmessage"))
            {
                scopes.Add(new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "Api for sending messages",
                    DisplayName = "InnovationBoost Send Message",
                    ScopeName = "innovationboost.sendmessage",
                    ShowInDiscoveryDocument = false,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>()
                });
            }

            if (!existingScopes.Any(m => m.ScopeName == "innovationboost.createnode"))
            {
                scopes.Add(new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "Api for creating Nodes",
                    DisplayName = "Create node API",
                    ScopeName = "innovationboost.createnode",
                    ShowInDiscoveryDocument = false,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>()
                });
            }

            if (scopes.Any())
            {
                await _outbackDbContext.Scopes.AddRangeAsync(scopes);
                await _outbackDbContext.SaveChangesAsync();
            }
        }

        private async Task CreateDefaultOpenIdScopes()
        {
            var existingScopes = await _outbackDbContext.Scopes.ToListAsync();

            var scopes = new List<OutbackScope>();
            if (!existingScopes.Any(m => m.ScopeName == "openid"))
            {
                scopes.Add(new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "OpenId Connect Subject identifier",
                    DisplayName = "openid",
                    ScopeName = "openid",
                    ShowInDiscoveryDocument = true,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>
                    {
                        new OutbackScopeClaim
                        {
                            Description = "Identifier for the End-User at the Issuer",
                            Type = StandardClaims.Subject
                        }
                    }
                });
            }

            if (!existingScopes.Any(m => m.ScopeName == "openid"))
            {
                scopes.Add(new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "Default profile claims",
                    DisplayName = "profile",
                    ScopeName = "profile",
                    ShowInDiscoveryDocument = true,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>
                    {
                        new OutbackScopeClaim
                        {
                            Description = "Given name(s) or first name(s) of the End-User.Note that in some cultures, people can have multiple given names; all can be present, with the names being separated by space characters.",
                            Type = StandardClaims.GivenName
                        },
                        new OutbackScopeClaim
                        {
                            Description = "Surname(s) or last name(s) of the End-User.Note that in some cultures, people can have multiple family names or no family name; all can be present, with the names being separated by space characters.",
                            Type = StandardClaims.FamilyName
                        }
                    }
                });
            }

            if (!existingScopes.Any(m => m.ScopeName == "openid"))
            {
                scopes.Add(new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "Email claims",
                    DisplayName = "email",
                    ScopeName = "email",
                    ShowInDiscoveryDocument = true,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>
                    {
                        new OutbackScopeClaim
                        {
                            Description = "End-User's preferred e-mail address. Its value MUST conform to the RFC 5322 [RFC5322] addr-spec syntax. The RP MUST NOT rely upon this value being unique, as discussed in Section 5.7.",
                            Type = StandardClaims.Email
                        },
                        new OutbackScopeClaim
                        {
                            Description = "True if the End-User's e-mail address has been verified; otherwise false. When this Claim Value is true, this means that the OP took affirmative steps to ensure that this e-mail address was controlled by the End-User at the time the verification was performed. The means by which an e-mail address is verified is context-specific, and dependent upon the trust framework or contractual agreements within which the parties are operating.",
                            Type = StandardClaims.EmailVerified
                        }
                    }
                });
            }

            if (scopes.Any())
            {
                await _outbackDbContext.Scopes.AddRangeAsync(scopes);
                await _outbackDbContext.SaveChangesAsync();
            }
        }

        private async Task CreateDefaultClientFamilies()
        {
            var existingClientFamilies = await _outbackDbContext.ClientFamilies.ToListAsync();

            var clientFamilies = new List<OutbackClientFamily>();

            if (!existingClientFamilies.Any(m => m.Name == "WebApplication"))
            {
                clientFamilies.Add(new OutbackClientFamily
                {
                    Name = "WebApplication",
                    Description = "Web applications"
                });
            }

            if (!existingClientFamilies.Any(m => m.Name == "SpaApplication"))
            {
                clientFamilies.Add(new OutbackClientFamily
                {
                    Name = "SpaApplication",
                    Description = "Browser SPA technology based applications"
                });
            }

            if (!existingClientFamilies.Any(m => m.Name == "Node"))
            {
                clientFamilies.Add(new OutbackClientFamily
                {
                    Name = "Node",
                    Description = "Home control nodes"
                });
            }
            
            if (clientFamilies.Any())
            {
                await _outbackDbContext.ClientFamilies.AddRangeAsync(clientFamilies);
                await _outbackDbContext.SaveChangesAsync();
            }
        }
    }

    public class Credentials
    {
        public string SpaApplicationClientId { get; set; }

        public string ClientId { get; set; }

        public string Secret { get; set; }

    }
}
