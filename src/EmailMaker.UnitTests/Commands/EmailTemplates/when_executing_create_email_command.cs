﻿using CoreDdd.Domain.Repositories;
using EmailMaker.Commands.Handlers;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.EmailTemplates;
using EmailMaker.Domain.Emails;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.UnitTests.Commands.EmailTemplates
{
    [TestFixture]
    public class when_executing_create_email_command
    {
        private IRepository<Email> _emailRepository;
        private IRepository<EmailTemplate> _emailTemplateRepository;
        private bool _eventRaised;

        [SetUp]
        public void Context()
        {
            _emailRepository = A.Fake<IRepository<Email>>();
            _emailTemplateRepository = A.Fake<IRepository<EmailTemplate>>();
            const int emailTemplateId = 23;
            A.CallTo(() => _emailTemplateRepository.Get(emailTemplateId)).Returns(new EmailTemplate(123));

            var handler = new CreateEmailCommandHandler(_emailRepository, _emailTemplateRepository);
            handler.CommandExecuted += args => _eventRaised = true;
            handler.ExecuteAsync(new CreateEmailCommand { EmailTemplateId = emailTemplateId}).Wait();
        }

        [Test]
        public void email_was_saved()
        {
            A.CallTo(() => _emailRepository.SaveAsync(A<Email>._)).MustHaveHappened();
        }

        [Test]
        public void event_was_raised()
        {
            _eventRaised.ShouldBe(true);
        }
    }
}