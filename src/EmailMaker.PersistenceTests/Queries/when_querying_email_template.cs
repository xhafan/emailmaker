﻿using System.Collections.Generic;
using System.Linq;
using CoreDdd.Nhibernate.TestHelpers;
using EmailMaker.Domain.EmailTemplates;
using EmailMaker.Dtos.EmailTemplates;
using EmailMaker.Queries.Handlers;
using EmailMaker.Queries.Messages;
using EmailMaker.TestHelper.Builders;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.PersistenceTests.Queries
{
    [TestFixture]
    public class when_querying_email_template : BasePersistenceTest
    {
        private EmailTemplate _emailTemplate;
        private IEnumerable<EmailTemplateDto> _result;

        [SetUp]
        public void Context()
        {
            _persistEmailTemplate();

            var queryHandler = new GetEmailTemplateQueryHandler(UnitOfWork);
            _result = queryHandler.Execute<EmailTemplateDto>(new GetEmailTemplateQuery {EmailTemplateId = _emailTemplate.Id});

            void _persistEmailTemplate()
            {
                var user = UserBuilder.New.Build();
                UnitOfWork.Save(user);
                _emailTemplate = EmailTemplateBuilder.New
                    .WithInitialHtml("html")
                    .WithName("name")
                    .WithUserId(user.Id)
                    .Build();
                var anotherEmailTemplate = EmailTemplateBuilder.New
                    .WithInitialHtml("another html")
                    .WithName("template name")
                    .WithUserId(user.Id)
                    .Build();
                UnitOfWork.Save(_emailTemplate);
                UnitOfWork.Save(anotherEmailTemplate);
            }
        }

        [Test]
        public void email_template_correctly_retrieved()
        {
            _result.Count().ShouldBe(1);
            var retrievedEmailTemplateDto = _result.First();
            retrievedEmailTemplateDto.EmailTemplateId.ShouldBe(_emailTemplate.Id);
            retrievedEmailTemplateDto.Name.ShouldBe(_emailTemplate.Name);
        }
    }
}