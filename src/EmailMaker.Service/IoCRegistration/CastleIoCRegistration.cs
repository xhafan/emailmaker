using Castle.Facilities.TypedFactory;
using Castle.Windsor;
using Castle.Windsor.Installer;
using CoreDdd.Nhibernate.Register.Castle;
using CoreDdd.Register.Castle;
using CoreIoC;
using CoreIoC.Castle;
using EmailMaker.Infrastructure.Register.Castle;
using EmailMaker.Queries.Register.Castle;
using EmailMaker.Service.EmailSenders;
using Rebus.CastleWindsor;

namespace EmailMaker.Service.IoCRegistration
{
    public static class CastleIoCRegistration
    {
        public static IWindsorContainer RegisterServicesIntoIoC()
        {
            var windsorContainer = new WindsorContainer();
            NhibernateInstaller.SetUnitOfWorkLifeStyle(x => x.PerRebusMessage());

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
    }
}
