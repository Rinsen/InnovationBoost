//using Microsoft.AspNet.Mvc;
//using Microsoft.AspNet.Mvc.Filters;

//namespace Rinsen.IdentityProvider
//{
//    public class ClaimsAuthorizationAttribute : AuthorizationFilterAttribute
//    {
//        public string ClaimType { get; set; }
//        public string ClaimValue { get; set; }

//        public override void OnAuthorization(AuthorizationContext context)
//        {
//            if (!context.HttpContext.User.HasClaim(ClaimType, ClaimValue))
//            {
//                context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
//            }
//        }
//    }
//}
