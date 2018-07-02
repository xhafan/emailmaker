#if NETCOREAPP
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
#endif

#if NETFRAMEWORK 
using System.Web.Mvc;
#endif

namespace EmailMaker.Controllers.BaseController
{
    [Authorize]
    public class AuthenticatedController : Controller
    {
        protected int GetUserId()
        {
#if NETCOREAPP
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            return userId;
#endif
#if NETFRAMEWORK
            return int.Parse(User.Identity.Name.Split('|')[1]);
#endif
        }

        protected string GetUserEmailAddress()
        {
#if NETCOREAPP
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmailAddress = claimsIdentity.FindFirst(ClaimTypes.Email).Value;
            return userEmailAddress;
#endif
#if NETFRAMEWORK
            return User.Identity.Name.Split('|')[0];
#endif
        }
    }
}
