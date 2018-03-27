using CoreDdd.Commands;
using EmailMaker.Commands.Handlers;
using Ninject;
using Ninject.Modules;
using Ninject.Extensions.Conventions;

namespace EmailMaker.Commands.Register.Ninject
{
    public class CommandHandlerBindings : NinjectModule
    {
        public override void Load()
        {
#if NET40
            Kernel.Bind(x => x
#else
            KernelConfiguration.Bind(x => x
#endif
                .FromAssemblyContaining<CreateEmailCommandHandler>()
                .SelectAllClasses()
                .InheritedFrom(typeof(ICommandHandler<>))
                .BindDefaultInterface()
                .Configure(y => y.InTransientScope()));
        }
    }
}