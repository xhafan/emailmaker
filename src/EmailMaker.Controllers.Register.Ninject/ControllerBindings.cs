using System.Web.Mvc;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace EmailMaker.Controllers.Register.Ninject
{
    public class ControllerBindings : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(x => x
                .FromAssemblyContaining<EmailController>()
                .SelectAllClasses()
                .InheritedFrom<IController>()
                .BindAllInterfaces()
                .Configure(y => y.InTransientScope()));
        }
    }
}