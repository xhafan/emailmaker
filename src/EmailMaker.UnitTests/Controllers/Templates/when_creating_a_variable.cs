using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Queries;
using EmailMaker.Commands.Messages;
using EmailMaker.Controllers;
using EmailMaker.Dtos.Users;
using EmailMaker.Queries.Messages;
using FakeItEasy;
using NUnit.Framework;

namespace EmailMaker.UnitTests.Controllers.Templates
{
    [TestFixture]
    public class when_creating_a_variable
    {
        private ICommandExecutor _commandExecutor;
        private IQueryExecutor _queryExecutor;
         
        private CreateVariableCommand _createVariableCommand;

        [SetUp]
        public async Task Context()
        {
            _commandExecutor = A.Fake<ICommandExecutor>();
            _queryExecutor = A.Fake<IQueryExecutor>();
            A.CallTo(() => _queryExecutor.ExecuteAsync<GetUserDetailsByEmailAddressQuery, UserDto>(A<GetUserDetailsByEmailAddressQuery>._))
                .Returns(new[] { new UserDto() });
            var controller = new TemplateController(_commandExecutor, _queryExecutor);
            _createVariableCommand = new CreateVariableCommand();
            
            await controller.CreateVariable(_createVariableCommand);
        }

        [Test]
        public void command_was_executed()
        {
            A.CallTo(() => _commandExecutor.ExecuteAsync(_createVariableCommand)).MustHaveHappened();
        }
    }
}