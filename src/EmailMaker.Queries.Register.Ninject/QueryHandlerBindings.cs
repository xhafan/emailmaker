using CoreDdd.Queries;
using EmailMaker.Queries.Handlers;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace EmailMaker.Queries.Register.Ninject
{
    public class QueryHandlerBindings : NinjectModule
    {
        public override void Load()
        {
#if NET40
            Kernel.Bind(x => x
#else
            KernelConfiguration.Bind(x => x
#endif
                .FromAssemblyContaining<GetEmailQueryHandler>()
                .SelectAllClasses()
                .InheritedFrom(typeof(IQueryHandler<>))
                .BindAllInterfaces()
                .Configure(y => y.InTransientScope()));
        }
    }
}