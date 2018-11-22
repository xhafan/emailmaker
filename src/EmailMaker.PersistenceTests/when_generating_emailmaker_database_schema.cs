using CoreDdd.Nhibernate.DatabaseSchemaGenerators;
using EmailMaker.Infrastructure;
using NUnit.Framework;

namespace EmailMaker.PersistenceTests
{
    [TestFixture]
    public class when_generating_emailmaker_database_schema
    {
        [Test]
        public void schema_is_generated_without_an_error()
        {
            new DatabaseSchemaGenerator(@".\schema.sql", new EmailMakerNhibernateConfigurator(shouldMapDtos: false)).Generate();
        }
    }
}
