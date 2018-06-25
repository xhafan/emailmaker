using CoreDdd.Nhibernate.Register.Ninject;
using CoreDdd.Register.Ninject;
using CoreIoC;
using CoreIoC.Ninject;
using EmailMaker.Infrastructure.Register.Ninject;
using EmailMaker.Queries.Register.Ninject;
using EmailMaker.Service.EmailSenders;
using Ninject;

namespace EmailMaker.Service.IoCRegistration
{
    public static class NinjectIoCRegistration
    {
        public static IKernel RegisterServicesIntoIoC()
        {
            var kernel = new StandardKernel();
            NhibernateBindings.SetUnitOfWorkLifeStyle(x => x.InThreadScope()); // todo: here should be "per rebus message" (see PerRebusMessage for CastleIoCRegistration)

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
