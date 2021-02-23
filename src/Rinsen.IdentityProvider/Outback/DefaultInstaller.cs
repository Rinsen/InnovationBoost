using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.Outback.Claims;

namespace Rinsen.IdentityProvider.Outback
{
    public class DefaultInstaller
    {
        private readonly OutbackDbContext _outbackDbContext;

        public DefaultInstaller(OutbackDbContext outbackDbContext)
        {
            _outbackDbContext = outbackDbContext;
        }

        public async Task<Credentials> Install()
        {
            await CreateDefaultOpenIdScopes();
            await CreateDefaultInnovationBoostScopes();
            
            // Create client for innovation boost web
            // Create client for innovation boost SPA application






        }

        private async Task CreateDefaultInnovationBoostScopes()
        {
            var scopes = new List<OutbackScope>
            {
                new OutbackScope
                {
                    ClaimsInIdToken = false,
                    Description = "Api for creating logs",
                    DisplayName = "Log service API",
                    ScopeName = "innovationboost.createlogs",
                    ShowInDiscoveryDocument = false,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>()
                },
                new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "Api for InnovationBoost administration",
                    DisplayName = "InnovationBoost Administration",
                    ScopeName = "innovationboost.administration",
                    ShowInDiscoveryDocument = false,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>()
                },
                new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "Api for session management",
                    DisplayName = "InnovationBoost Session management",
                    ScopeName = "innovationboost.sessions",
                    ShowInDiscoveryDocument = false,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>()
                },
                new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "Api for sending messages",
                    DisplayName = "InnovationBoost Send Message",
                    ScopeName = "innovationboost.sendmessage",
                    ShowInDiscoveryDocument = false,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>()
                },
                new OutbackScope
                {
                    ClaimsInIdToken = true,
                    Description = "Api for creating Nodes",
                    DisplayName = "Create node API",
                    ScopeName = "innovationboost.createnode",
                    ShowInDiscoveryDocument = false,
                    Enabled = true,
                    ScopeClaims = new List<OutbackScopeClaim>()
                }
            };

            await _outbackDbContext.OutbackScopes.AddRangeAsync(scopes);
            await _outbackDbContext.SaveChangesAsync();
        }

        private async Task CreateDefaultOpenIdScopes()
        {
            var scopes = new List<OutbackScope>
            {
                new OutbackScope 
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
                            Type = OpenIdStandardClaims.Subject
                        } 
                    } 
                },
                new OutbackScope
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
                            Type = OpenIdStandardClaims.GivenName
                        },
                        new OutbackScopeClaim
                        {
                            Description = "Surname(s) or last name(s) of the End-User.Note that in some cultures, people can have multiple family names or no family name; all can be present, with the names being separated by space characters.",
                            Type = OpenIdStandardClaims.FamilyName
                        }
                    }
                },
                new OutbackScope
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
                            Type = OpenIdStandardClaims.Email
                        },
                        new OutbackScopeClaim
                        {
                            Description = "True if the End-User's e-mail address has been verified; otherwise false. When this Claim Value is true, this means that the OP took affirmative steps to ensure that this e-mail address was controlled by the End-User at the time the verification was performed. The means by which an e-mail address is verified is context-specific, and dependent upon the trust framework or contractual agreements within which the parties are operating.",
                            Type = OpenIdStandardClaims.EmailVerified
                        }
                    }
                }
            };

            await _outbackDbContext.OutbackScopes.AddRangeAsync(scopes);
            await _outbackDbContext.SaveChangesAsync();
        }
    }

    public class Credentials
    {
        public string ClientId { get; set; }

        public string Secret { get; set; }

    }
}
