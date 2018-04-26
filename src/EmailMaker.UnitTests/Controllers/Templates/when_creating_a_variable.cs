using EmailMaker.Commands.Messages;
using EmailMaker.Controllers;
using FakeItEasy;
using NUnit.Framework;

namespace EmailMaker.UnitTests.Controllers.Templates
{
    [TestFixture]
    public class when_creating_a_variable : BaseEmailmakerControllerTest
    {
        private CreateVariableCommand _createVariableCommand;

        [SetUp]
        public override void Context()
        {
            base.Context();

            var controller = new TemplateController(CommandExecutor, null);
            _createVariableCommand = new CreateVariableCommand();
            controller.CreateVariable(_createVariableCommand).Wait();
        }

        [Test]
        public void command_was_executed()
        {
            A.CallTo(() => CommandExecutor.ExecuteAsync(_createVariableCommand)).MustHaveHappened();
        }
    }
}