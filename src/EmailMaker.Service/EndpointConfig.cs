﻿using Castle.Windsor;
using Castle.Windsor.Installer;
using CoreDdd.Nhibernate.Configurations;
using CoreDdd.Nhibernate.Register.Castle;
using CoreDdd.Register.Castle;
using CoreIoC;
using CoreIoC.Castle;
using EmailMaker.Infrastructure.Register.Castle;
using EmailMaker.Messages;
using EmailMaker.Queries.Register.Castle;
using EmailMaker.Service.Handlers;
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
                                                typeof (SendEmailForEmailRecipientMessage).Assembly,
                                                typeof (SendEmailForEmailRecipientMessageHandler).Assembly
                                            };
            SetLoggingLibrary.Log4Net(log4net.Config.XmlConfigurator.Configure);
            var windsorContainer = new WindsorContainer();
            Configure
                .With(nserviceBusAssemblies)
                .CastleWindsorBuilder(windsorContainer)
                .BinarySerializer()
                .MsmqTransport()
                .DisableTimeoutManager()
                .MsmqSubscriptionStorage()
                .UnicastBus();
            NhibernateInstaller.SetUnitOfWorkLifeStyle(x => x.PerThread);
            windsorContainer.Install(
                FromAssembly.Containing<QueryExecutorInstaller>(),
                FromAssembly.Containing<EmailSenderInstaller>(),
                FromAssembly.Containing<QueryHandlerInstaller>(),
                FromAssembly.Containing<NhibernateInstaller>(),
                FromAssembly.Containing<EmailMakerNhibernateInstaller>()
                );            
            IoC.Initialize(new CastleContainer(windsorContainer));
            
            ConfigureNhibernate();            
        }

        private void ConfigureNhibernate()
        {
            IoC.Resolve<INhibernateConfigurator>();
        }
    }
}
