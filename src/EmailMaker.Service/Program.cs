using System;
using System.IO;
using Castle.MicroKernel.Registration;
using CoreDdd.Nhibernate.Configurations;
using CoreIoC;
using EmailMaker.Infrastructure;
using EmailMaker.Messages;
using EmailMaker.Service.Handlers;
using EmailMaker.Service.IoCRegistration;
using Microsoft.Extensions.Configuration;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Ninject;
using Rebus.Routing.TypeBased;
using Ninject.Extensions.Conventions;
using Rebus.Bus;

namespace EmailMaker.Service
{
    class Program
    {
        private static IConfigurationRoot _configuration;

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
                    rebusConfigurer.Transport(t => t.UseMsmq(rebusInputQueue));
                    break;
#endif
                case "RabbitMQ":
                    rebusConfigurer.Transport(t =>
                        t.UseRabbitMq(_configuration["Rebus:RabbitMQ:ConnectionString"], rebusInputQueue));
                    break;
                default:
                    throw new Exception($"Unknown rebus transport: {rebusTransport}");
            }

            return rebusConfigurer
                .Routing(r => r.TypeBased().MapAssemblyOf<SendEmailForEmailRecipientMessage>(rebusInputQueue))
                .Options(o =>
                {
                    o.EnableUnitOfWork(
                        RebusUnitOfWork.Create,
                        RebusUnitOfWork.Commit,
                        RebusUnitOfWork.Rollback,
                        RebusUnitOfWork.Cleanup
                    );
                })
                .Start();
        }

        private static void _LoadConfiguration()
        {
            _configuration = AppSettings.Configuration;
        }

        private static IHandlerActivator GetCastleWindsorHandlerActivator()
        {
            var container = CastleIoCRegistration.RegisterServicesIntoIoC();
            container.Register(
                Classes.FromAssemblyContaining<EmailEnqueuedToBeSentEventMessageHandler>()
                    .BasedOn<IHandleMessages>()
                    .WithServiceAllInterfaces()
                    .LifestyleTransient()
            );
            return new CastleWindsorContainerAdapter(container);
        }

        private static IHandlerActivator GetNinjectHandlerActivator()
        {
            var kernel = NinjectIoCRegistration.RegisterServicesIntoIoC();           
            kernel.Bind(x => x
                .FromAssemblyContaining<EmailEnqueuedToBeSentEventMessageHandler>()
                .SelectAllClasses()
                .InheritedFrom<IHandleMessages>()
                .BindAllInterfaces()
                .Configure(y => y.InTransientScope())
            );
            return new NinjectContainerAdapter(kernel);
        }

        private static void _ConfigureNhibernate()
        {
            IoC.Resolve<INhibernateConfigurator>();
        }
    }
}
