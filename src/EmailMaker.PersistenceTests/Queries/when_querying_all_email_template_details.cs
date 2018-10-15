using System.Collections.Generic;
using System.Linq;
using CoreDdd.Nhibernate.TestHelpers;
using EmailMaker.Domain.EmailTemplates;
using EmailMaker.Domain.Users;
using EmailMaker.Dtos.EmailTemplates;
using EmailMaker.Queries.Handlers;
using EmailMaker.Queries.Messages;
using EmailMaker.TestHelper.Builders;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.PersistenceTests.Queries
{
    [TestFixture]
    public class when_querying_all_email_template_details : BasePersistenceTest
    {
        private IEnumerable<EmailTemplateDetailsDto> _result;
        private EmailTemplate _emailTemplate;
        private User _user;

        [SetUp]
        public void Context()
        {
            _persistEmailTemplate();

            var queryHandler = new GetAllEmailTemplateQueryHandler(PersistenceTestHelper.UnitOfWork);
            _result = queryHandler.Execute<EmailTemplateDetailsDto>(new GetAllEmailTemplateQuery { UserId = _user.Id });

            void _persistEmailTemplate()
            {
                _user = UserBuilder.New.Build();
                Save(_user);
                _emailTemplate = EmailTemplateBuilder.New
                    .WithInitialHtml("html")
                    .WithName("template name")
                    .WithUserId(_user.Id)
                    .Build();
                Save(_emailTemplate);
            }
        }

        [Test]
        public void email_template_correctly_retrieved()
        {
            var retrievedEmailTemplateDto = _result.Single();
            retrievedEmailTemplateDto.EmailTemplateId.ShouldBe(_emailTemplate.Id);
        }
    }
}