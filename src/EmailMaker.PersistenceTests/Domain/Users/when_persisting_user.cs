using CoreDdd.Nhibernate.TestHelpers;
using EmailMaker.Domain.Users;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.PersistenceTests.Domain.Users
{
    [TestFixture]
    public class when_persisting_user : BasePersistenceTest
    {
        private const string FirstName = "first name";
        private const string LastName = "last name";
        private const string EmailAddress = "email address";
        private const string Password = "password";
        private User _user;
        private User _retrievedUser;

        [SetUp]
        public void Context()
        {
            _user = new User(FirstName, LastName, EmailAddress, Password);
            UnitOfWork.Save(_user);
            UnitOfWork.Clear();

            _retrievedUser = UnitOfWork.Get<User>(_user.Id);
        }

        [Test]
        public void user_correctly_retrieved()
        {
            _retrievedUser.FirstName.ShouldBe(FirstName);
            _retrievedUser.LastName.ShouldBe(LastName);
            _retrievedUser.EmailAddress.ShouldBe(EmailAddress);
            _retrievedUser.Password.ShouldBe(Password);
        }
    }
}
