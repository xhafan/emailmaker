using System.Diagnostics;
using CoreDdd.Queries;
using EmailMaker.Controllers.BaseController;
using EmailMaker.Controllers.ViewModels;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#endif

#if NETFRAMEWORK 
using System.Web.Mvc;
#endif

namespace EmailMaker.Controllers
{
    public class HomeController : AuthenticatedController
    {
        public HomeController(IQueryExecutor queryExecutor)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

#if NETCOREAPP
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
#endif
    }
}
