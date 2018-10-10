using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Threading;
using Castle.Windsor;
using Castle.Windsor.Installer;
using CoreDdd.Nhibernate.Configurations;
using CoreDdd.Nhibernate.Register.Castle;
using CoreIoC;
using CoreIoC.Castle;
using DatabaseBuilder;
using EmailMaker.Infrastructure.Register.Castle;
using Npgsql;
using NUnit.Framework;

namespace EmailMaker.PersistenceTests
{
    [SetUpFixture]
    public class RunOncePerTestRun
    {
        private Mutex _mutex;
        private WindsorContainer _windsorContainer;

        [OneTimeSetUp]
        public void SetUp()
        {
            _acquireSynchronizationMutex();

            CoreDddNhibernateInstaller.SetUnitOfWorkLifeStyle(x => x.PerThread);

            _windsorContainer = new WindsorContainer();
            _windsorContainer.Install(
                FromAssembly.Containing<CoreDddNhibernateInstaller>(),
                FromAssembly.Containing<EmailMakerNhibernateInstaller>()
            );
            IoC.Initialize(new CastleContainer(_windsorContainer));

            _buildDatabase();

            void _acquireSynchronizationMutex()
            {
                var mutexName = GetType().Namespace;
                _mutex = new Mutex(false, mutexName);
                if (!_mutex.WaitOne(TimeSpan.FromSeconds(60)))
                {
                    throw new Exception(
                        "Timeout waiting for synchronization mutex to prevent other .net frameworks running concurrent tests over the same database");
                }
            }

            void _buildDatabase()
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

        private string _GetAssemblyLocation()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var indexOfLastBackslash = assemblyLocation.LastIndexOf(Path.DirectorySeparatorChar);
            return assemblyLocation.Substring(0, indexOfLastBackslash);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _windsorContainer.Dispose();

            _mutex.ReleaseMutex();
            _mutex.Dispose();
        }
    }
}