#if NETCOREAPP
using System.Diagnostics;
using EmailMaker.Controllers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmailMaker.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
#endif