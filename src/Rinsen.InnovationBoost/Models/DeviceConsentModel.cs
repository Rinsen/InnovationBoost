//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using IdentityServer4.Models;

//namespace Rinsen.InnovationBoost.Models
//{
//    public class DeviceConsentModel
//    {
//        public bool HasUserCode { get { return !string.IsNullOrEmpty(UserCode); } }

//        public string ClientName { get; set; }

//        public string UserCode { get; set; }
//        public string ClientUrl { get; internal set; }
//        public string ClientLogoUrl { get; internal set; }
//        public bool AllowRememberConsent { get; internal set; }
//        public List<IdentityResource> IdentityScope { get; internal set; } = new List<IdentityResource>();

//        public List<Scope> ResourceScope { get; internal set; } = new List<Scope>();


//    }
//}
