using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#endif

#if NETFRAMEWORK 
using System.Web.Mvc;
#endif

namespace EmailMaker.Controllers.Register.Castle
{
    public class ControllerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes
                                   .FromAssemblyContaining<EmailController>()
                                   .BasedOn<ControllerBase>()
                                   .Configure(x => x.LifestyleTransient()));
        }
    }
}