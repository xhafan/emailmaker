﻿using Core.Domain;
using EmailMaker.Commands.Handlers;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.Emails;
using EmailMaker.Domain.EmailTemplates;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace EmailMaker.Commands.Tests.EmailTemplates
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
            _emailRepository = MockRepository.GenerateMock<IRepository<Email>>();
            _emailTemplateRepository = MockRepository.GenerateStub<IRepository<EmailTemplate>>();
            var emailTemplateId = 23;
            _emailTemplateRepository.Stub(a => a.GetById(emailTemplateId)).Return(new EmailTemplate());

            var handler = new CreateEmailCommandHandler(_emailRepository, _emailTemplateRepository);
            handler.CommandExecuted += (sender, args) => _eventRaised = true;
            handler.Execute(new CreateEmailCommand { EmailTemplateId = emailTemplateId});
        }

        [Test]
        public void email_was_saved()
        {
            _emailRepository.AssertWasCalled(a => a.Save(Arg<Email>.Is.NotNull));
        }

        [Test]
        public void event_was_raised()
        {
            _eventRaised.ShouldBe(true);
        }

    }
}