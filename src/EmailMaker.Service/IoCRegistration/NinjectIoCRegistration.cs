using CoreDdd.Nhibernate.Register.Ninject;
using CoreDdd.Register.Ninject;
using CoreIoC;
using CoreIoC.Ninject;
using EmailMaker.Infrastructure.Register.Ninject;
using EmailMaker.Queries.Register.Ninject;
using Ninject;

namespace EmailMaker.Service.IoCRegistration
{
    public static class NinjectIoCRegistration
    {
        public static IKernel RegisterIoCServices()
        {
            var kernel = new StandardKernel();
            NhibernateBindings.SetUnitOfWorkLifeStyle(x => x.InThreadScope());

            kernel.Load(
                typeof(QueryExecutorBindings).Assembly,
                typeof(EmailSenderBindings).Assembly,
                typeof(QueryHandlerBindings).Assembly,
                typeof(NhibernateBindings).Assembly,
                typeof(EmailMakerNhibernateBindings).Assembly
            );
            IoC.Initialize(new NinjectContainer(kernel));
            return kernel;
        }
    }
}
