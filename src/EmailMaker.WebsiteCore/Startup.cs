using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Castle.Facilities.AspNetCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using CoreDdd.AspNetCore.Middleware;
using CoreDdd.Domain.Events;
using CoreDdd.Nhibernate.Configurations;
using CoreDdd.Nhibernate.Register.Castle;
using CoreDdd.Register.Castle;
using CoreIoC;
using CoreIoC.Castle;
using DatabaseBuilder;
using EmailMaker.Commands.Register.Castle;
using EmailMaker.Controllers;
using EmailMaker.Controllers.Register.Castle;
using EmailMaker.Domain.Register.Castle;
using EmailMaker.Infrastructure;
using EmailMaker.Infrastructure.Register.Castle;
using EmailMaker.Queries.Register.Castle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Npgsql;
using Rebus.Config;
using Rebus.TransactionScopes;

namespace EmailMaker.WebsiteCore
{
    //  ASP.NET Core with Castle Windsor integration - https://github.com/castleproject/Windsor/blob/master/docs/aspnetcore-facility.md
    public class Startup
    {
        private readonly WindsorContainer _windsorContainer = new WindsorContainer();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Setup component model contributors for making windsor services available to IServiceProvider
            _windsorContainer.AddFacility<AspNetCoreFacility>(f => f.CrossWiresInto(services));

            // https://stackoverflow.com/a/49047358/379279
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = "/Account/LogOn";
                        options.LogoutPath = "/Account/LogOff";
                    });

            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); // avoid json camel case, https://stackoverflow.com/a/38202543/379279
            services.AddLogging(lb => lb.AddConsole().AddDebug());

            // Custom application component registrations, ordering is important here
            _RegisterApplicationComponents();

            // Castle Windsor integration, controllers, tag helpers and view components, this should always come after RegisterApplicationComponents
            return services.AddWindsor(_windsorContainer,
                opts => opts.UseEntryAssembly(typeof(HomeController).Assembly),
                () => services.BuildServiceProvider(validateScopes: false)); // <- Optional
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            _windsorContainer.GetFacility<AspNetCoreFacility>().RegistersMiddlewareInto(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Error");
            }

            var unitOfWorkMode = AppSettings.Configuration["UnitOfWorkMode"];
            switch (unitOfWorkMode)
            {
                case "TransactionScopeUnitOfWork":
                    _setupTransactionScopeUnitOfWork();
                    break;
                case "UnitOfWork":
                    _setupUnitOfWorkWithDelayedDomainEventHandling();
                    break;
                default:
                    throw new NotSupportedException("Unsupported unit of work mode.");
            }

            var pathBase = AppSettings.Configuration["PathBase"];
            if (!string.IsNullOrWhiteSpace(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            _ConfigureBus(_windsorContainer);
            _UpgradeDatabase();

            void _setupTransactionScopeUnitOfWork()
            {
                _windsorContainer.Register(
                    Component.For<TransactionScopeUnitOfWorkMiddleware>()
                        .DependsOn(Dependency.OnValue<System.Transactions.IsolationLevel>(
                            System.Transactions.IsolationLevel.ReadCommitted
                            ))
                        .DependsOn(Dependency.OnValue<Action<System.Transactions.TransactionScope>>(
                            (Action<System.Transactions.TransactionScope>)(transactionScope => transactionScope.EnlistRebus())
                            ))
                        .LifestyleSingleton().AsMiddleware()
                );

                DomainEvents.Initialize(_windsorContainer.Resolve<IDomainEventHandlerFactory>());
            }

            void _setupUnitOfWorkWithDelayedDomainEventHandling()
            {
                _windsorContainer.Register(
                    Component.For<UnitOfWorkMiddleware>()
                        .DependsOn(Dependency.OnValue<IsolationLevel>(IsolationLevel.ReadCommitted))
                        .LifestyleSingleton().AsMiddleware()
                );

                DomainEvents.Initialize(
                    _windsorContainer.Resolve<IDomainEventHandlerFactory>(),
                    isDelayedDomainEventHandlingEnabled: true
                    );
            }
        }

        private void _RegisterApplicationComponents()
        {
            CoreDddNhibernateInstaller.SetUnitOfWorkLifeStyle(x => x.Scoped());

            _windsorContainer.Install(
                FromAssembly.Containing<CoreDddInstaller>(),
                FromAssembly.Containing<CoreDddNhibernateInstaller>(),
                FromAssembly.Containing<ControllerInstaller>(),
                FromAssembly.Containing<CommandHandlerInstaller>(),
                FromAssembly.Containing<EventHandlerInstaller>(),
                FromAssembly.Containing<QueryHandlerInstaller>(),
                FromAssembly.Containing<EmailMakerNhibernateInstaller>()
            );
            
            IoC.Initialize(new CastleContainer(_windsorContainer));
        }

        private void _ConfigureBus(WindsorContainer container)
        {
            var rebusInputQueueName = AppSettings.Configuration["Rebus:InputQueueName"];
            var rebusRabbitMqConnectionString = AppSettings.Configuration["Rebus:RabbitMQ:ConnectionString"];

            Rebus.Config.Configure.With(new CastleWindsorContainerAdapter(container))
                .Transport(t => t.UseRabbitMq(rebusRabbitMqConnectionString, rebusInputQueueName))
                .Start();
        }

        private void _UpgradeDatabase()
        {
            var configuration = IoC.Resolve<INhibernateConfigurator>().GetConfiguration();

            var connectionString = configuration.Properties["connection.connection_string"];
            var connectionDriverClass = configuration.Properties["connection.driver_class"];
            var dbProviderName = _GetDbProviderName(connectionDriverClass);

            var assemblyLocation = _GetAssemblyLocation();
            var folderWithSqlFiles = Path.Combine(assemblyLocation, "EmailMaker.Database", dbProviderName);

            var builderOfDatabase = new BuilderOfDatabase(_getDbConnection);
            builderOfDatabase.BuildDatabase(folderWithSqlFiles);

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
                    throw new Exception($"Unsupported NHibernate connection.driver_class: {connectionDriverClass}");
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

        private void OnShutdown()
        {
            _windsorContainer.Dispose();
        }
    }
}
