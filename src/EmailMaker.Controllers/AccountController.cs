using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Queries;
using EmailMaker.Commands.Messages;
using EmailMaker.Controllers.ViewModels;
using EmailMaker.Dtos.Users;
using EmailMaker.Queries.Messages;

#if NETCOREAPP
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endif

#if NETFRAMEWORK 
using System.Web.Mvc;
using System.Web.Security;
#endif

namespace EmailMaker.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IQueryExecutor _queryExecutor;

        public AccountController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var message = new GetUserDetailsByEmailAddressQuery {EmailAddress = model.EmailAddress};
                var userDto = (await _queryExecutor.ExecuteAsync<GetUserDetailsByEmailAddressQuery, UserDto>(message)).FirstOrDefault();

                if (userDto != null)
                {
                    if (userDto.Password.Equals(model.Password.Trim()))
                    {
#if NETCOREAPP
                        var identity = new ClaimsIdentity(GetUserClaims(userDto), CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTime.UtcNow.AddDays(2) // todo: configure this value
                        });
#endif
#if NETFRAMEWORK
                        FormsAuthentication.SetAuthCookie(_GetAuthenticationCookieUserName(userDto), model.RememberMe);
#endif
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");

            return View(model);
        }

#if NETCOREAPP
        private IEnumerable<Claim> GetUserClaims(UserDto user)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.EmailAddress));
            claims.Add(new Claim(ClaimTypes.Email, user.EmailAddress));
            return claims;
        }
#endif

#if NETFRAMEWORK
        private string _GetAuthenticationCookieUserName(UserDto userDto)
        {
            return userDto.EmailAddress + "|" + userDto.UserId;
        }        
#endif

#if NETCOREAPP
        public async Task<ActionResult> LogOff()
#endif
#if NETFRAMEWORK
        public ActionResult LogOff()
#endif
        {
#if NETCOREAPP
            await HttpContext.SignOutAsync();
#endif
#if NETFRAMEWORK
            FormsAuthentication.SignOut();
#endif
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var createdUserId = default(int);
                var command = new CreateUserCommand { EmailAddress = model.Email, Password = model.Password };
                _commandExecutor.CommandExecuted += args => createdUserId = (int)args.Args;
                await _commandExecutor.ExecuteAsync(command);

                var userDto = new UserDto
                {
                    UserId = createdUserId,
                    EmailAddress = model.Email
                };

#if NETCOREAPP
                var identity = new ClaimsIdentity(GetUserClaims(userDto), CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
#endif
#if NETFRAMEWORK
                FormsAuthentication.SetAuthCookie(_GetAuthenticationCookieUserName(userDto), false);
#endif
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var command = new ChangePasswordForUserCommand
                {
                    UserId = GetUserId(),
                    OldPassword = model.OldPassword,
                    NewPassword = model.NewPassword
                };
                await _commandExecutor.ExecuteAsync(command);

                return RedirectToAction("ChangePasswordSuccess");
            }

            return View(model);
        }

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

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
    }
}
