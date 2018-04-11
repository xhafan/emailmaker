using CoreDdd.Domain.Events;
using EmailMaker.Domain.EventHandlers;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace EmailMaker.Domain.Register.Ninject
{
    public class EventHandlerBindings : NinjectModule
    {
        public override void Load()
        {
#if NET40
            Kernel.Bind(x => x
#else
            KernelConfiguration.Bind(x => x
#endif
                .FromAssemblyContaining<EmailEnqueuedToBeSentDomainEventHandler>()
                .SelectAllClasses()
                .InheritedFrom(typeof(IDomainEventHandler<>))
                .BindAllInterfaces()
                .Configure(y => y.InTransientScope()));
        }
    }
}