using CoreDdd.Nhibernate.DatabaseSchemaGenerators;

namespace EmailMaker.Infrastructure
{
    public class EmailMakerDatabaseSchemaGenerator : DatabaseSchemaGenerator
    {
        public EmailMakerDatabaseSchemaGenerator(string databaseSchemaFileName)
            : base(
                databaseSchemaFileName,
                new EmailMakerNhibernateConfigurator(shouldMapDtos: false)
            )
        {
        }
    }
}
