using CoreDdd.Commands;
using EmailMaker.Commands.Handlers;
using Ninject.Modules;
using Ninject.Extensions.Conventions;

namespace EmailMaker.Commands.Register.Ninject
{
    public class CommandHandlerBindings : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(x => x
                .FromAssemblyContaining<CreateEmailCommandHandler>()
                .SelectAllClasses()
                .InheritedFrom(typeof(ICommandHandler<>))
                .BindAllInterfaces()
                .Configure(y => y.InTransientScope()));
        }
    }
}