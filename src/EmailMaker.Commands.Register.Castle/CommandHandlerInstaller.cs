using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CoreDdd.Commands;
using EmailMaker.Commands.Handlers;

namespace EmailMaker.Commands.Register.Castle
{
    public class CommandHandlerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes
                    .FromAssemblyContaining<CreateEmailCommandHandler>()
                    .BasedOn(typeof(ICommandHandler<>))
                    .WithService.FirstInterface()
                    .Configure(x => x.LifestyleTransient()));
        }
    }
}