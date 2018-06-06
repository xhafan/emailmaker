using Ninject.Extensions.Conventions;
using Ninject.Modules;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#endif

#if NETFRAMEWORK 
using System.Web.Mvc;
#endif

namespace EmailMaker.Controllers.Register.Ninject
{
    public class ControllerBindings : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(x => x
                .FromAssemblyContaining<EmailController>()
                .SelectAllClasses()
                .InheritedFrom<ControllerBase>()
                .BindAllInterfaces()
                .Configure(y => y.InTransientScope()));
        }
    }
}