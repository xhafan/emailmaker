﻿using RebusTestExtensions.Extensions;
using EmailMaker.Domain.EventHandlers;
using EmailMaker.Domain.Events.Emails;
using EmailMaker.Messages;
using FakeItEasy;
using NUnit.Framework;
using Rebus.Bus;
using Shouldly;

namespace EmailMaker.UnitTests.Domain.EventHandlers
{
    [TestFixture]
    public class when_handling_enqueue_email_to_be_sent_event
    {
        private IBus _bus;
        private EmailEnqueuedToBeSentEventMessage _message;
        private const int EmailId = 23;

        [SetUp]
        public void Context()
        {
            _bus = A.Fake<IBus>();
            _bus.ExpectMessagePublished<EmailEnqueuedToBeSentEventMessage>(x => _message = x);

            var evnt = new EmailEnqueuedToBeSentDomainEvent{ EmailId = EmailId };
            var handler = new EmailEnqueuedToBeSentDomainEventHandler(_bus);
            handler.Handle(evnt);
        }

        [Test]
        public void message_was_sent()
        {
            _message.ShouldNotBeNull();
            _message.EmailId.ShouldBe(EmailId);
        }
    }
}
