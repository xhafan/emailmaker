using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using CoreDdd.AspNet.HttpModules;
using CoreDdd.Domain.Events;
using CoreDdd.Nhibernate.Configurations;
using CoreDdd.Nhibernate.Register.Castle;
using CoreDdd.Register.Castle;
using CoreDdd.UnitOfWorks;
using CoreWeb;
using CoreWeb.ModelBinders;
using EmailMaker.Commands.Register.Castle;
using EmailMaker.Controllers.Register.Castle;
using EmailMaker.Domain.Register.Castle;
using EmailMaker.Infrastructure.Register.Castle;
using EmailMaker.Queries.Register.Castle;
using Npgsql;
using Rebus.Config;
using Rebus.Persistence.FileSystem;
using Rebus.TransactionScopes;

namespace EmailMaker.Website
{
    public class EmailMakerWebsiteApplication : HttpApplication
    {
        private WindsorContainer _windsorContainer;

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

            _windsorContainer = new WindsorContainer();
            _configureBus(_windsorContainer);

            CoreDddNhibernateInstaller.SetUnitOfWorkLifeStyle(x => x.PerWebRequest);

            _windsorContainer.Install(
                FromAssembly.Containing<CoreDddInstaller>(),
                FromAssembly.Containing<CoreDddNhibernateInstaller>(),
                FromAssembly.Containing<ControllerInstaller>(),
                FromAssembly.Containing<CommandHandlerInstaller>(),
                FromAssembly.Containing<EventHandlerInstaller>(),
                FromAssembly.Containing<QueryHandlerInstaller>(),
                FromAssembly.Containing<EmailMakerNhibernateInstaller>()
                );

            _setupTransactionScopeUnitOfWork();
            //_setupDelayedDomainEventHandlingForUnitOfWork();

            ControllerBuilder.Current.SetControllerFactory(new IoCControllerFactory(_windsorContainer));
            ModelBinders.Binders.DefaultBinder = new EnumConverterModelBinder();

            _UpgradeDatabase();

            void _setupTransactionScopeUnitOfWork()
            {
                // call this method only when TransactionScopeUnitOfWorkHttpModule is used instead of UnitOfWorkHttpModule (see web.config system.webServer -> modules)

                TransactionScopeUnitOfWorkHttpModule.Initialize(
                    _windsorContainer.Resolve<IUnitOfWorkFactory>(),
                    transactionScopeEnlistmentAction: transactionScope => transactionScope.EnlistRebus()
                );

                DomainEvents.Initialize(_windsorContainer.Resolve<IDomainEventHandlerFactory>());
            }

            void _setupDelayedDomainEventHandlingForUnitOfWork()
            {
                // call this method only when UnitOfWorkHttpModule is used instead of TransactionScopeUnitOfWorkHttpModule (see web.config system.webServer -> modules)

                UnitOfWorkHttpModule.Initialize(_windsorContainer.Resolve<IUnitOfWorkFactory>());

                DomainEvents.Initialize(
                    _windsorContainer.Resolve<IDomainEventHandlerFactory>(),
                    isDelayedDomainEventHandlingEnabled: true
                );
            }

            void _configureBus(WindsorContainer container)
            {
                var rebusConfigurer = Configure.With(new CastleWindsorContainerAdapter(container));

                var rebusInputQueueName = ConfigurationManager.AppSettings["RebusInputQueueName"];
                var rebusTransport = ConfigurationManager.AppSettings["RebusTransport"];

                switch (rebusTransport)
                {
                    case "MSMQ":
                        rebusConfigurer
                            .Transport(x => x.UseMsmq(rebusInputQueueName))
                            .Subscriptions(x => x.UseJsonFile($"{Path.GetTempPath()}\\emailmaker_msmq_subscriptions.json"))
                            ;
                        break;
                    case "RabbitMQ":
                        var rebusRabbitMqConnectionString = ConfigurationManager.AppSettings["RebusRabbitMqConnectionString"];
                        rebusConfigurer.Transport(x => x.UseRabbitMq(rebusRabbitMqConnectionString, rebusInputQueueName));
                        break;
                    default:
                        throw new Exception($"Unknown rebus transport: {rebusTransport}");
                }

                rebusConfigurer.Start();
            }
        }

        protected void Application_End()
        {
            _windsorContainer.Dispose();
        }

        private void _UpgradeDatabase()
        {
            var configuration = _windsorContainer.Resolve<INhibernateConfigurator>().GetConfiguration();

            var connectionString = configuration.Properties["connection.connection_string"];
            var connectionDriverClass = configuration.Properties["connection.driver_class"];
            var dbProviderName = _GetDbProviderName(connectionDriverClass);

            var assemblyLocation = _GetAssemblyLocation();
            var folderWithSqlFiles = Path.Combine(assemblyLocation, "EmailMaker.Database", dbProviderName);

            var databaseBuilder = new DatabaseBuilder.DatabaseBuilder(_getDbConnection);
            databaseBuilder.UpgradeDatabase(folderWithSqlFiles);

            IDbConnection _getDbConnection()
            {

                switch (dbProviderName)
                {
                    case string x when x.Contains("sqlite"):
                        return new SQLiteConnection(connectionString);
                    case string x when x.Contains("sqlserver"):
                        return new SqlConnection(connectionString);
                    case string x when x.Contains("postgresql"):
                        return new NpgsqlConnection(connectionString);
                    default:
                        throw new Exception("Unsupported NHibernate connection.driver_class");
                }
            }
        }

        private string _GetDbProviderName(string connectionDriverClass)
        {
            switch (connectionDriverClass)
            {
                case string x when x.Contains("Npgsql"):
                    return "postgresql";
                case string x when x.Contains("SQLite"):
                    return "sqlite";
                case string x when x.Contains("SqlClient"):
                    return "sqlserver";
                default:
                    throw new Exception("Unsupported NHibernate connection.driver_class");
            }
        }

        // https://stackoverflow.com/a/283917/379279
        private string _GetAssemblyLocation()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}