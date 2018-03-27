using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EmailMaker.Controllers.Register.Castle
{
    public class ControllerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes
                                   .FromAssemblyContaining<EmailController>()
                                   .BasedOn<IController>()
                                   .Configure(x => x.LifestyleTransient()));
        }
    }
}