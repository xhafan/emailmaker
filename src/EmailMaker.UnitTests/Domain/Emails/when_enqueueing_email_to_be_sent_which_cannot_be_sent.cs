using System;
using EmailMaker.Domain.Emails;
using EmailMaker.Domain.Emails.EmailStates;
using EmailMaker.TestHelper.Builders;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.UnitTests.Domain.Emails
{
    [TestFixture]
    public class when_enqueueing_email_to_be_sent_which_cannot_be_sent
    {
        private Email _email;
        private const string FromAddress = "from address";
        private const string Subject = "subject";
        private Exception _exception;

        [SetUp]
        public void Context()
        {
            var template = EmailTemplateBuilder.New.Build();
            var state = A.Fake<EmailState>();
            A.CallTo(() => state.CanSend).Returns(false);
            A.CallTo(() => state.Name).Returns("state");
            _email = new EmailBuilder()
                .WithEmailTemplate(template)
                .WithState(state)
                .Build();
            _exception = Should.Throw<Exception>(() => _email.EnqueueEmailToBeSent(FromAddress, null, Subject));
        }

        [Test]
        public void correct_exception_is_thrown()
        {
            _exception.Message.ShouldBe("cannot enqeue email in the current state: state");
        }
    }
}