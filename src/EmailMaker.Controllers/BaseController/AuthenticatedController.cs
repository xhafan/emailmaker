using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreDdd.Queries;
using EmailMaker.Dtos.Users;
using EmailMaker.Queries.Messages;

#if NETCOREAPP
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
#endif

#if NETFRAMEWORK 
using System.Web.Mvc;
using System.Web.Routing;
#endif

namespace EmailMaker.Controllers.BaseController
{
    //[Authorize] todo
    public class AuthenticatedController : Controller
    {
        private readonly IQueryExecutor _queryExecutor;

        public AuthenticatedController(IQueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        protected async Task<int> GetUserId()
        {
            // todo: fix this user id retrieval via cookie persistence or in identity
//            var message = new GetUserDetailsByEmailAddressQuery {EmailAddress = User.Identity.Name};
//            var user = (await _queryExecutor.ExecuteAsync<GetUserDetailsByEmailAddressQuery, UserDto>(message)).Single();
//            return user.UserId;
            return -1;
        }

        //within a controller or base controller
        private void SetRouteValues(string action, string controller, RouteValueDictionary routeValues)
        {
            if (routeValues != null)
            {
                foreach (var key in routeValues.Keys)
                {
                    RouteData.Values[key] = routeValues[key];
                }
            }

            RouteData.Values["action"] = action;
            RouteData.Values["controller"] = controller;
        }

        // todo: remove this method
        protected RedirectToRouteResult RedirectToAction<TController>(Expression<Func<TController, object>> actionExpression) where TController : Controller
        {
            var controllerName = typeof(TController).GetControllerName();
            var actionName = actionExpression.GetActionName();
            var routeValues = actionExpression.GetRouteValues();

            SetRouteValues(actionName, controllerName, routeValues);

            return new RedirectToRouteResult("Default", RouteData.Values);
        }
    }
}
