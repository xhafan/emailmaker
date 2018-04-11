using System.Data;
using CoreDdd.UnitOfWorks;
using FakeItEasy;
using NUnit.Framework;

namespace EmailMaker.Service.Tests.UnitOfWorkMessageModules
{
    [TestFixture]
    public class when_handling_begin_message
    {
        private IUnitOfWork _unitOfWork;
        private UnitOfWorkMessageModule _module;

        [SetUp]
        public virtual void Context()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _module = new UnitOfWorkMessageModule(_unitOfWork);

            _module.HandleBeginMessage();
        }

        [Test]
        public void unit_of_work_begins_transaction()
        {
            A.CallTo(() => _unitOfWork.BeginTransaction(IsolationLevel.Unspecified)).MustHaveHappened();
        }
    }
}