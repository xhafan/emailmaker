﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Transactions;
using Castle.Facilities.AspNetCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using CoreDdd.Domain.Events;
using CoreDdd.Nhibernate.Configurations;
using CoreDdd.Nhibernate.Register.Castle;
using CoreDdd.Register.Castle;
using CoreIoC;
using CoreIoC.Castle;
using CoreUtils.Storages;
using EmailMaker.Commands.Register.Castle;
using EmailMaker.Controllers;
using EmailMaker.Controllers.Register.Castle;
using EmailMaker.Domain.Register.Castle;
using EmailMaker.Infrastructure;
using EmailMaker.Infrastructure.Register.Castle;
using EmailMaker.Messages;
using EmailMaker.Queries.Register.Castle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EmailMaker.WebsiteCore.Middleware;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Npgsql;
using Rebus.Config;
using Rebus.Routing.TypeBased;

namespace EmailMaker.WebsiteCore
{
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
            app.UseMiddlewareFromWindsor<TransactionScopeUnitOfWorkMiddleware>(_windsorContainer);
            //app.UseMiddlewareFromWindsor<UnitOfWorkMiddleware>(_windsorContainer);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void _RegisterApplicationComponents()
        {
            _configureBus(_windsorContainer);

            NhibernateInstaller.SetUnitOfWorkLifeStyle(x => x.Scoped());

            _windsorContainer.Install(
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

            IoC.Initialize(new CastleContainer(_windsorContainer));

            _UpgradeDatabase();

            void _registerTransactionScopeStoragePerWebRequest()
            {
                _windsorContainer.Register(
                    Component.For<IStorage<TransactionScope>>()
                        .ImplementedBy<Storage<TransactionScope>>()
                        .LifestyleScoped());
            }

            // this is needed only when UnitOfWorkMiddleware is used instead of TransactionScopeUnitOfWorkMiddleware
            void _registerDelayedDomainEventHandlingActionsStoragePerWebRequest() 
            {
                _windsorContainer.Register(
                    Component.For<IStorage<DelayedDomainEventHandlingActions>>()
                        .ImplementedBy<Storage<DelayedDomainEventHandlingActions>>()
                        .LifestyleScoped());
            }

            void _configureBus(WindsorContainer container)
            {
                var rebusInputQueueName = AppSettings.Configuration["Rebus:InputQueueName"];
                var rebusEmailMakerServiceQueueName = AppSettings.Configuration["Rebus:EmailMakerServiceQueueName"];
                var rebusRabbitMqConnectionString = AppSettings.Configuration["Rebus:RabbitMQ:ConnectionString"];

                Rebus.Config.Configure.With(new CastleWindsorContainerAdapter(container))
                    .Transport(t => t.UseRabbitMq(rebusRabbitMqConnectionString, rebusInputQueueName))
                    .Routing(r => r.TypeBased().MapAssemblyOf<EmailEnqueuedToBeSentEventMessage>(rebusEmailMakerServiceQueueName))
                    .Start();
            }
        }

        private void _UpgradeDatabase()
        {
            var configuration = IoC.Resolve<INhibernateConfigurator>().GetConfiguration();

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
    }
}
