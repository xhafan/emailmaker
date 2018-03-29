using Castle.Windsor;
using Castle.Windsor.Installer;
using CoreDdd.Nhibernate.Register.Castle;
using CoreDdd.Register.Castle;
using CoreIoC;
using CoreIoC.Castle;
using EmailMaker.Infrastructure.Register.Castle;
using EmailMaker.Queries.Register.Castle;
using NServiceBus;

namespace EmailMaker.Service.IoCRegistration
{
    public static class CastleIoCRegistration
    {
        public static IWindsorContainer RegisterIoCServices()
        {
            var windsorContainer = new WindsorContainer();
            NhibernateInstaller.SetUnitOfWorkLifeStyle(x => x.PerThread);
            windsorContainer.Install(
                FromAssembly.Containing<QueryExecutorInstaller>(),
                FromAssembly.Containing<EmailSenderInstaller>(),
                FromAssembly.Containing<QueryHandlerInstaller>(),
                FromAssembly.Containing<NhibernateInstaller>(),
                FromAssembly.Containing<EmailMakerNhibernateInstaller>()
            );
            IoC.Initialize(new CastleContainer(windsorContainer));
            return windsorContainer;
        }

        public static Configure Builder(this Configure config, IWindsorContainer container)
        {
            return config.CastleWindsorBuilder(container);
        }
    }
}
