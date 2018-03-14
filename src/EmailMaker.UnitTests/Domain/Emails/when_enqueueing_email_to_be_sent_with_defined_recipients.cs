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
    public class when_enqueueing_email_to_be_sent_with_defined_recipients
    {
        private Exception _exception;

        [SetUp]
        public void Context()
        {
            var template = EmailTemplateBuilder.New.Build();
            var state = A.Fake<EmailState>();
            A.CallTo(() => state.CanSend).Returns(true);
            var email = new EmailBuilder()
                .WithEmailTemplate(template)
                .WithState(state)
                .WithRecipient(new Recipient("email", "name"))
                .Build();

            _exception = Should.Throw<Exception>(() => email.EnqueueEmailToBeSent(null, null, null));
        }

        [Test]
        public void correct_exception_is_thrown()
        {
            _exception.Message.ShouldBe("recipients must be empty");
        }
    }
}