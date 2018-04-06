using System.Collections.Generic;
using System.Linq;
using CoreDdd.Nhibernate.TestHelpers;
using EmailMaker.Domain.Emails;
using EmailMaker.Dtos.Emails;
using EmailMaker.Queries.Handlers;
using EmailMaker.Queries.Messages;
using EmailMaker.TestHelper.Builders;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.PersistenceTests.Queries
{
    [TestFixture]
    public class when_querying_email : BasePersistenceTest
    {
        private Email _email;
        private IEnumerable<EmailDto> _result;

        [SetUp]
        public void Context()
        {
            _persistEmail();

            var queryHandler = new GetEmailQueryHandler();
            _result = queryHandler.Execute<EmailDto>(new GetEmailQuery { EmailId = _email.Id });

            void _persistEmail()
            {
                var user = UserBuilder.New.Build();
                Save(user);
                var emailTemplate = EmailTemplateBuilder.New
                    .WithInitialHtml("html")
                    .WithName("template name")
                    .WithUserId(user.Id)
                    .Build();
                _email = new Email(emailTemplate);
                var anotherEmailTemplate = EmailTemplateBuilder.New
                    .WithInitialHtml("another html")
                    .WithName("template name")
                    .WithUserId(user.Id)
                    .Build();
                var anotherEmail = new Email(anotherEmailTemplate);

                Save(emailTemplate);
                Save(_email);
                Save(anotherEmailTemplate);
                Save(anotherEmail);
            }
        }

        [Test]
        public void email_template_correctly_retrieved()
        {
            _result.Count().ShouldBe(1);
            var retrievedEmailDto = _result.First();
            retrievedEmailDto.EmailId.ShouldBe(_email.Id);
        }
    }
}