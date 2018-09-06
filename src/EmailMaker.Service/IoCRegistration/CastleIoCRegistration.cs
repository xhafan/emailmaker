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
            CoreDddNhibernateInstaller.SetUnitOfWorkLifeStyle(x => x.PerRebusMessage());

            windsorContainer.Install(
                FromAssembly.Containing<CoreDddInstaller>(),
                FromAssembly.Containing<CoreDddNhibernateInstaller>(),
                FromAssembly.Containing<EmailSenderInstaller>(),
                FromAssembly.Containing<QueryHandlerInstaller>(),
                FromAssembly.Containing<EmailMakerNhibernateInstaller>()
            );
            IoC.Initialize(new CastleContainer(windsorContainer));
            return windsorContainer;
        }
    }
}
