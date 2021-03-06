using System;
using EmailMaker.TestHelper.Builders;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.UnitTests.Domain.Users
{
    [TestFixture]
    public class when_changing_password_with_incorrect_old_password
    {
        private Exception _exception;

        [SetUp]
        public void Context()
        {
            var user = UserBuilder.New.Build();
            _exception = Should.Throw<Exception>(() => user.ChangePassword("incorrect old password", "new password"));
        }

        [Test]
        public void password_was_changed()
        {
            _exception.Message.ShouldBe("Old password does not match");
        }
    }
}