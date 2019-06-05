using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerDeviceCode
    {

        public DateTime CreationTime { get; set; }
       
        public int Lifetime { get; set; }
        
        public string ClientId { get; set; }
        
        public bool IsOpenId { get; set; }
        
        public bool IsAuthorized { get; set; }
        
        public string RequestedScopes { get; set; }
        
        public string AuthorizedScopes { get; set; }
        

    }
}
