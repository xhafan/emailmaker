using System.Transactions;
using CoreDdd.Nhibernate.Configurations;
using CoreIoC;
using EmailMaker.Service.Handlers;
using EmailMaker.Service.IoCRegistration;
using NServiceBus;

namespace EmailMaker.Service
{
    public class EndpointConfig : AsA_Server, IConfigureThisEndpoint, IWantCustomInitialization
    {
        public void Init()
        {
            var nserviceBusAssemblies = new[]
                                            {
                                                typeof (IMessage).Assembly,
                                                typeof (Configure).Assembly,
                                                typeof (EmailEnqueuedToBeSentEventMessageHandler).Assembly
                                            };
            SetLoggingLibrary.Log4Net(log4net.Config.XmlConfigurator.Configure);

            var container = CastleIoCRegistration.RegisterIoCServices(); // change it to NinjectIoCRegistration to use Ninject IoC
            Configure
                .With(nserviceBusAssemblies)
                .Builder(container)
                .BinarySerializer()
                .MsmqTransport()
                .IsolationLevel(IsolationLevel.ReadCommitted)
                .IsTransactional(true)
                .DisableTimeoutManager()
                .MsmqSubscriptionStorage()                
                .UnicastBus();
           
            ConfigureNhibernate();            
        }

        private void ConfigureNhibernate()
        {
            IoC.Resolve<INhibernateConfigurator>();
        }
    }
}
