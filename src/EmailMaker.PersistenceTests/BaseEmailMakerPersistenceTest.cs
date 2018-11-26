using CoreDdd.Nhibernate.UnitOfWorks;
using EmailMaker.Infrastructure;
using NUnit.Framework;

namespace EmailMaker.PersistenceTests
{
    public abstract class BasePersistenceTest
    {
        protected NhibernateUnitOfWork UnitOfWork;

        protected BasePersistenceTest()
        {
            UnitOfWork = new NhibernateUnitOfWork(new EmailMakerNhibernateConfigurator());
        }

        [SetUp]
        public void TestFixtureSetUp()
        {
            UnitOfWork.BeginTransaction();
        }

        [TearDown]
        public void TestFixtureTearDown()
        {
            UnitOfWork.Rollback();
        }
    }
}