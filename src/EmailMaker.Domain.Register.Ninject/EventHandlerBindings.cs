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
            Kernel.Bind(x => x
                .FromAssemblyContaining<EmailEnqueuedToBeSentDomainEventHandler>()
                .SelectAllClasses()
                .InheritedFrom(typeof(IDomainEventHandler<>))
                .BindAllInterfaces()
                .Configure(y => y.InTransientScope()));
        }
    }
}