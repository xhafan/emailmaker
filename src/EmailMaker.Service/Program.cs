using System;
using Castle.MicroKernel.Registration;
using CoreDdd.Nhibernate.Configurations;
using CoreIoC;
using EmailMaker.Messages;
using EmailMaker.Service.Handlers;
using EmailMaker.Service.IoCRegistration;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Ninject;
using Rebus.Routing.TypeBased;
using Ninject.Extensions.Conventions;

namespace EmailMaker.Service
{
    class Program
    {
        static void Main()
        {
            var handlerActivator = GetCastleWindsorHandlerActivator();
            //var handlerActivator = GetNinjectHandlerActivator();

            _ConfigureNhibernate();

            Configure.With(handlerActivator)
                .Transport(t => t.UseMsmq("EmailMaker.Service"))
                .Routing(r => r.TypeBased().MapAssemblyOf<SendEmailForEmailRecipientMessage>("EmailMaker.Service"))
                .Options(o => {
                    o.EnableUnitOfWork(
                        RebusUnitOfWork.Create, 
                        RebusUnitOfWork.Commit, 
                        RebusUnitOfWork.Rollback, 
                        RebusUnitOfWork.Cleanup
                        );
                })
                .Start();

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }

        private static IHandlerActivator GetCastleWindsorHandlerActivator()
        {
            var container = CastleIoCRegistration.RegisterIoCServices();
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
            var kernel = NinjectIoCRegistration.RegisterIoCServices();           
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
