//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;

//namespace Rinsen.InnovationBoost.Controllers
//{
//    [Authorize("AdminsOnly")]
//    public class AdminController : Controller
//    {
//        private readonly IdentityServerDefaultInstaller _identityServerDefaultInstaller;
//        private readonly IConfiguration _configuration;

//        public AdminController(IdentityServerDefaultInstaller identityServerDefaultInstaller,
//            IConfiguration configuration
//            )
//        {
//            _identityServerDefaultInstaller = identityServerDefaultInstaller;
//            _configuration = configuration;
//        }

//        [AllowAnonymous]
//        public async Task<IActionResult> InstallDefault()
//        {
//            var credentials = await _identityServerDefaultInstaller.Install();

//            return Ok(credentials);
//        }

//        public async Task<IActionResult> Diagnostics()
//        {
//            try
//            {
//                using (var httpClient = new HttpClient())
//                {
//                    var disco = await httpClient.GetDiscoveryDocumentAsync(_configuration["Rinsen:InnovationBoost"]);

//                    if (disco.IsError)
//                        throw new Exception(disco.Error);

//                    var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
//                    {
//                        Address = disco.TokenEndpoint,
//                        ClientId = _configuration["Rinsen:ClientId"],
//                        ClientSecret = _configuration["Rinsen:ClientSecret"]
//                    });

//                    if (tokenResponse.IsError)
//                    {
//                        throw new Exception(tokenResponse.Error);
//                    }

//                    return Ok(tokenResponse.AccessToken);
//                }
//            }
//            catch (Exception e)
//            {
//                return Ok(e.Message + e.StackTrace);
//            }
//        }
//    }
//}
