using System.Security.Claims;

#if NETCOREAPP
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endif

#if NETFRAMEWORK 
using System.Web.Mvc;
using System.Web.Routing;
#endif

namespace EmailMaker.Controllers.BaseController
{
#if NETCOREAPP // for now only force authentication for ASP.NET Core app
    [Authorize]
#endif
    public class AuthenticatedController : Controller
    {
        protected int GetUserId()
        {
#if NETCOREAPP // for now only force authentication for ASP.NET Core app
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            return userId;
#endif
#if NETFRAMEWORK
            return -1;
#endif
        }
    }
}
