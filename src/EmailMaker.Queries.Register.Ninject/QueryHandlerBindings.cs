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
            Kernel.Bind(x => x
                .FromAssemblyContaining<GetEmailQueryHandler>()
                .SelectAllClasses()
                .InheritedFrom(typeof(IQueryHandler<>))
                .BindAllInterfaces()
                .Configure(y => y.InTransientScope()));
        }
    }
}