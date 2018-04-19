using System.Collections.Generic;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using CoreDdd.Domain.Events;
using CoreDdd.Nhibernate.Register.Castle;
using CoreDdd.Register.Castle;
using CoreIoC;
using CoreIoC.Castle;
using CoreUtils.Storages;
using CoreWeb;
using CoreWeb.ModelBinders;
using EmailMaker.Commands.Register.Castle;
using EmailMaker.Controllers.Register.Castle;
using EmailMaker.Domain.Register.Castle;
using EmailMaker.Infrastructure.Register.Castle;
using EmailMaker.Queries.Register.Castle;
using NServiceBus;

namespace EmailMaker.Website
{
    public class UnitOfWorkApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var nserviceBusAssemblies = new[]
                                            {
                                                typeof (IMessage).Assembly,
                                                typeof (Configure).Assembly,
                                            };
            var windsorContainer = new WindsorContainer();
            Configure
                .With(nserviceBusAssemblies)
                .Log4Net()
                .CastleWindsorBuilder(windsorContainer)
                .BinarySerializer()
                .MsmqTransport()
                .UnicastBus()
                .LoadMessageHandlers()
                .CreateBus()
                .Start();

            NhibernateInstaller.SetUnitOfWorkLifeStyle(x => x.PerWebRequest);

            windsorContainer.Install(
                FromAssembly.Containing<ControllerInstaller>(),
                FromAssembly.Containing<QueryExecutorInstaller>(),
                FromAssembly.Containing<CommandHandlerInstaller>(),
                FromAssembly.Containing<EventHandlerInstaller>(),
                FromAssembly.Containing<QueryHandlerInstaller>(),
                FromAssembly.Containing<NhibernateInstaller>(),
                FromAssembly.Containing<EmailMakerNhibernateInstaller>()
                );

            _registerTransactionScopeStoragePerWebRequest();
            _registerDelayedDomainEventHandlingActionsStoragePerWebRequest();

            IoC.Initialize(new CastleContainer(windsorContainer));

            ControllerBuilder.Current.SetControllerFactory(new IoCControllerFactory());
            ModelBinders.Binders.DefaultBinder = new EnumConverterModelBinder();

            void _registerTransactionScopeStoragePerWebRequest()
            {
                windsorContainer.Register(
                    Component.For<IStorage<TransactionScope>>()
                        .ImplementedBy<Storage<TransactionScope>>()
                        .LifeStyle.PerWebRequest);
            }

            void _registerDelayedDomainEventHandlingActionsStoragePerWebRequest()
            {
                windsorContainer.Register(
                    Component.For<IStorage<DelayedDomainEventHandlingActions>>()
                        .ImplementedBy<Storage<DelayedDomainEventHandlingActions>>()
                        .LifeStyle.PerWebRequest);
            }
        }
    }
}