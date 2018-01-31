using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace CoreDdd.Nhibernate.Tests.UnitOfWorks
{
    [TestFixture]
    public class when_rolling_transaction_back : NhibernateUnitOfWorkWithStartedTransactionSetup
    {
        [SetUp]
        public override void Context()
        {
            base.Context();
                       
            UnitOfWork.Rollback();
        }

        [Test]
        public void rollback_was_called_on_transaction()
        {
            Transaction.AssertWasCalled(x => x.Rollback());
        }

        [Test]
        public void transaction_is_disposed()
        {
            Transaction.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void session_was_disposed()
        {
            Session.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void session_was_set_to_null()
        {
            UnitOfWork.Session.ShouldBe(null);
        }

        [Test]
        public void unit_of_work_is_not_active()
        {
            UnitOfWork.IsActive().ShouldBe(false);
        }
    }
}