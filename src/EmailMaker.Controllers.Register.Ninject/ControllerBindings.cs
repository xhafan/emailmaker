using System.Web.Mvc;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace EmailMaker.Controllers.Register.Ninject
{
    public class ControllerBindings : NinjectModule
    {
        public override void Load()
        {
#if NET40
            Kernel.Bind(x => x
#else
            KernelConfiguration.Bind(x => x
#endif
                .FromAssemblyContaining<EmailController>()
                .SelectAllClasses()
                .InheritedFrom<IController>()
                .BindAllInterfaces()
                .Configure(y => y.InTransientScope()));
        }
    }
}