using gbajax.Security;
using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace gbajax
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public object WebconfigurationManager { get; private set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpRequest httprequest = HttpContext.Current.Request;
            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            string CookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();

            if(httprequest.Cookies[CookieName] != null)
            {
                JwtObject jwtObject = JWT.Decode<JwtObject>(Convert.ToString(httprequest.Cookies[CookieName].Value), Encoding.UTF8.GetBytes(SecretKey), JwsAlgorithm.HS512);
                string[] roles = jwtObject.Role.Split(new char[] { ',' });

                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, jwtObject.Account),
                    new Claim(ClaimTypes.NameIdentifier, jwtObject.Account)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieName);
                claimsIdentity.AddClaim(new Claim(@"http://schemas.microsoft.com/accesscontrolservice/20110/07/claims/identityprovider", "My Identity", @"http://www.w3.org/2001/XMLSchema#string"));
                HttpContext.Current.User = new GenericPrincipal(claimsIdentity, roles);
                Thread.CurrentPrincipal = HttpContext.Current.User;
                AntiForgeryConfig.UniqueClaimTypeIdentifier = System.Security.Claims.ClaimTypes.NameIdentifier;




            }
        }


    }
}
