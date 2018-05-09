using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
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
using EmailMaker.Messages;
using EmailMaker.Queries.Register.Castle;
using Npgsql;
using Rebus.Config;
using Rebus.Routing.TypeBased;

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

            var windsorContainer = new WindsorContainer();
            _configureBus(windsorContainer);

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

            _UpgradeDatabase();

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

            void _configureBus(WindsorContainer container)
            {
                Configure.With(new CastleWindsorContainerAdapter(container))
                    .Transport(t => t.UseMsmq("EmailMaker.Website"))
                    .Routing(r => r.TypeBased().MapAssemblyOf<EmailEnqueuedToBeSentEventMessage>("EmailMaker.Service"))
                    .Start();
            }
        }

        private void _UpgradeDatabase()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["EmailMakerConnection"];
            var connectionString = connectionStringSettings.ConnectionString;
            var dbProviderName = connectionStringSettings.ProviderName;

            var assemblyLocation = _GetAssemblyLocation();
            var folderWithSqlFiles = $"{assemblyLocation}\\EmailMaker.Database\\{dbProviderName}";

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