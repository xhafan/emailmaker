using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace CoreIoC.Tests.IoCs
{
    [TestFixture]
    public class when_resolving_all_services_generic : IoCSetup
    {
        private interface IServiceType { }
        private class ServiceTypeOne : IServiceType { }
        private class ServiceTypeTwo : IServiceType { }

        private IServiceType[] _result;
        private ServiceTypeOne _serviceTypeOne;
        private ServiceTypeTwo _serviceTypeTwo;


        [SetUp]
        public override void Context()
        {
            base.Context();
            _serviceTypeOne = new ServiceTypeOne();
            _serviceTypeTwo = new ServiceTypeTwo();
            Container.Stub(x => x.ResolveAll<IServiceType>()).Return(new IServiceType[] { _serviceTypeOne, _serviceTypeTwo });

            _result = IoC.ResolveAll<IServiceType>();
        }

        [Test]
        public void all_service_types_is_resolved()
        {
            _result.ShouldBe(new IServiceType[] { _serviceTypeOne, _serviceTypeTwo });
        }
    }
}