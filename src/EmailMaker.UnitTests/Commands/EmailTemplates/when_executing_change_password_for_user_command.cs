using CoreDdd.Domain.Repositories;
using EmailMaker.Commands.Handlers;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.Users;
using FakeItEasy;
using NUnit.Framework;

namespace EmailMaker.UnitTests.Commands.EmailTemplates
{
    [TestFixture]
    public class when_executing_change_password_for_user_command
    {
        private IRepository<User> _userRepository;
        private const int UserId = 23;
        private const string OldPassword = "old password";
        private const string NewPassword = "new password";
        private User _user;

        [SetUp]
        public void Context()
        {
            _user = A.Fake<User>();
            _userRepository = A.Fake<IRepository<User>>();
            A.CallTo(() => _userRepository.GetAsync(UserId)).Returns(_user);
            var handler = new ChangePasswordForUserCommandHandler(_userRepository);
            handler.ExecuteAsync(new ChangePasswordForUserCommand { UserId = UserId, OldPassword = OldPassword, NewPassword = NewPassword}).Wait();
        }

        [Test]
        public void password_was_changed()
        {
            A.CallTo(() => _user.ChangePassword(OldPassword, NewPassword)).MustHaveHappened();
        }

    }
}