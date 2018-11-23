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
            CoreDddNhibernateBindings.SetUnitOfWorkLifeStyle(x => x.InThreadScope()); // todo: here should be "per rebus message" (see PerRebusMessage for CastleIoCRegistration); might not play nicely with async code

            kernel.Load(
                typeof(CoreDddBindings).Assembly,
                typeof(CoreDddNhibernateBindings).Assembly,
                typeof(EmailSenderBindings).Assembly,
                typeof(QueryHandlerBindings).Assembly,
                typeof(EmailMakerNhibernateBindings).Assembly
            );
            IoC.Initialize(new NinjectContainer(kernel));
            return kernel;
        }
    }
}
