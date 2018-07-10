using System;
using EmailMaker.Infrastructure;

namespace EmailMaker.Tools
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Choose from the following options and press enter:");
            Console.WriteLine("1 Generate database schema sql file");

            var line = Console.ReadLine();
            if (line == "1")
            {
                var dbDriverName = _getDbDriverName();
                var databaseSchemaFileName = $"EmailMaker_generated_database_schema_{dbDriverName}.sql";
                new EmailMakerDatabaseSchemaGenerator(databaseSchemaFileName).Generate();
                Console.WriteLine($"Database schema sql file has been generated into {databaseSchemaFileName}");
            }

            string _getDbDriverName()
            {
                var configuration = new EmailMakerNhibernateConfigurator(shouldMapDtos: false).GetConfiguration();
                var connectionDriverClass = configuration.Properties["connection.driver_class"];

                switch (connectionDriverClass)
                {
                    case string x when x.Contains("SQLite"):
                        return "sqlite";
                    case string x when x.Contains("SqlClient"):
                        return "sqlserver";
                    case string x when x.Contains("NpgsqlDriver"):
                        return "postgresql";
                    default:
                        throw new Exception("Unsupported NHibernate connection.driver_class");
                }
            }

        }
    }
}
