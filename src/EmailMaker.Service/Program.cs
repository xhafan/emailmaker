using System;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CoreDdd.Nhibernate.Configurations;
using CoreDdd.Rebus.UnitOfWork;
using CoreDdd.UnitOfWorks;
using CoreIoC;
using EmailMaker.Infrastructure;
using EmailMaker.Messages;
using EmailMaker.Service.Handlers;
using EmailMaker.Service.IoCRegistration;
using Microsoft.Extensions.Configuration;
using Ninject;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Ninject;
using Rebus.Routing.TypeBased;
using Ninject.Extensions.Conventions;
using Rebus.Bus;
using Rebus.CastleWindsor;

#if NETFRAMEWORK
using Rebus.Persistence.FileSystem;
#endif

namespace EmailMaker.Service
{
    class Program
    {
        private static IConfigurationRoot _configuration;
        private static IWindsorContainer _windsorContainer;
        private static IKernel _kernel;

        static void Main()
        {
            _LoadConfiguration();
            var handlerActivator = _RegisterServicesIntoIoCAndGetHandlerActivator();
            _ConfigureNhibernate();
            using (_ConfigureAndStartRebus(handlerActivator))
            {
                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }

            _DisposeIoCContainer();
        }

        private static IHandlerActivator _RegisterServicesIntoIoCAndGetHandlerActivator()
        {
            IHandlerActivator handlerActivator;
            var iocContainer = _configuration["IoCContainer"];
            switch (iocContainer)
            {
                case "Castle":
                    handlerActivator = GetCastleWindsorHandlerActivator();
                    break;
                case "Ninject":
                    handlerActivator = GetNinjectHandlerActivator();
                    break;
                default:
                    throw new Exception($"Unknown IoC container: {iocContainer}");
            }

            return handlerActivator;
        }

        private static IBus _ConfigureAndStartRebus(IHandlerActivator handlerActivator)
        {
            var rebusConfigurer = Configure.With(handlerActivator);

            var rebusInputQueue = _configuration["Rebus:InputQueueName"];
            var rebusTransport = _configuration["Rebus:Transport"];
            switch (rebusTransport)
            {
#if NETFRAMEWORK
                case "MSMQ":
                    rebusConfigurer
                        .Transport(x => x.UseMsmq(rebusInputQueue))
                        .Subscriptions(x => x.UseJsonFile($"{Path.GetTempPath()}\\emailmaker_msmq_subscriptions.json"))
                        ;
                    break;
#endif
                case "RabbitMQ":
                    rebusConfigurer.Transport(x => x.UseRabbitMq(_configuration["Rebus:RabbitMQ:ConnectionString"], rebusInputQueue));
                    break;
                default:
                    throw new Exception($"Unknown rebus transport: {rebusTransport}");
            }

            var rebusUnitOfWorkMode = _configuration["Rebus:UnitOfWorkMode"];
            switch (rebusUnitOfWorkMode)
            {
                case "TransactionScopeUnitOfWork":
                    RebusTransactionScopeUnitOfWork.Initialize(
                        unitOfWorkFactory: IoC.Resolve<IUnitOfWorkFactory>(),
                        isolationLevel: System.Transactions.IsolationLevel.ReadCommitted,
                        transactionScopeEnlistmentAction: null
                    );
                    rebusConfigurer
                        .Options(o =>
                        {
                            o.EnableUnitOfWork(
                                RebusTransactionScopeUnitOfWork.Create,
                                RebusTransactionScopeUnitOfWork.Commit,
                                RebusTransactionScopeUnitOfWork.Rollback,
                                RebusTransactionScopeUnitOfWork.Cleanup
                            );
                        })
                        ;
                    break;
                case "UnitOfWork":
                    RebusUnitOfWork.Initialize(
                        unitOfWorkFactory: IoC.Resolve<IUnitOfWorkFactory>(),
                        isolationLevel: System.Data.IsolationLevel.ReadCommitted
                    );
                    rebusConfigurer
                        .Options(o =>
                        {
                            o.EnableUnitOfWork(
                                RebusUnitOfWork.Create,
                                RebusUnitOfWork.Commit,
                                RebusUnitOfWork.Rollback,
                                RebusUnitOfWork.Cleanup
                            );
                        })
                        ; break;
                default:
                    throw new Exception($"Unknown rebus unit of work mode: {rebusUnitOfWorkMode}");
            }

            var bus = rebusConfigurer.Start();
            bus.Subscribe<EmailEnqueuedToBeSentEventMessage>().Wait();
            return bus;

        }

        private static void _LoadConfiguration()
        {
            _configuration = AppSettings.Configuration;
        }

        private static IHandlerActivator GetCastleWindsorHandlerActivator()
        {
            _windsorContainer = CastleIoCRegistration.RegisterServicesIntoIoC();
            _windsorContainer.AutoRegisterHandlersFromAssemblyOf<EmailEnqueuedToBeSentEventMessageHandler>();
            return new CastleWindsorContainerAdapter(_windsorContainer);
        }

        private static IHandlerActivator GetNinjectHandlerActivator()
        {
            _kernel = NinjectIoCRegistration.RegisterServicesIntoIoC();           
            _kernel.Bind(x => x
                .FromAssemblyContaining<EmailEnqueuedToBeSentEventMessageHandler>()
                .SelectAllClasses()
                .InheritedFrom<IHandleMessages>()
                .BindAllInterfaces()
                .Configure(y => y.InTransientScope())
            );
            return new NinjectContainerAdapter(_kernel);
        }

        private static void _ConfigureNhibernate()
        {
            IoC.Resolve<INhibernateConfigurator>();
        }

        private static void _DisposeIoCContainer()
        {
            _windsorContainer?.Dispose();
            _kernel?.Dispose();
        }
    }
}
