using System.Collections.Generic;
using System.Linq;
using CoreDdd.Nhibernate.TestHelpers;
using EmailMaker.Domain.Emails;
using EmailMaker.Dtos;
using EmailMaker.Dtos.Emails;
using EmailMaker.Queries.Handlers;
using EmailMaker.Queries.Messages;
using EmailMaker.TestHelper.Builders;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.PersistenceTests.Queries
{
    [TestFixture]
    public class when_querying_email_parts : BasePersistenceTest
    {
        private IEnumerable<EmailPartDto> _result;
        private Email _email;

        [SetUp]
        public void Context()
        {
            _persistEmail();

            var queryHandler = new GetEmailPartsQueryHandler(UnitOfWork);
            _result = queryHandler.Execute<EmailPartDto>(new GetEmailPartsQuery { EmailId = _email.Id });

            void _persistEmail()
            {
                var user = UserBuilder.New.Build();
                UnitOfWork.Save(user);
                var emailTemplate = EmailTemplateBuilder.New
                    .WithInitialHtml("123")
                    .WithName("template name")
                    .WithUserId(user.Id)
                    .Build();
                UnitOfWork.Save(emailTemplate);
                emailTemplate.CreateVariable(emailTemplate.Parts.First().Id, 1, 1);
                UnitOfWork.Save(emailTemplate);

                var anotherEmailTemplate = EmailTemplateBuilder.New
                    .WithInitialHtml("another html")
                    .WithName("template name")
                    .WithUserId(user.Id)
                    .Build();
                UnitOfWork.Save(anotherEmailTemplate);

                _email = new Email(emailTemplate);
                UnitOfWork.Save(_email);
            }
        }

        [Test]
        public void email_template_parts_correctly_retrieved()
        {
            _result.Count().ShouldBe(3);

            var htmlPart = (HtmlEmailPart)_email.Parts.First();
            var partDto = _result.Single(x => x.PartId == htmlPart.Id);
            partDto.EmailId.ShouldBe(_email.Id);
            partDto.PartType.ShouldBe(PartType.Html);
            partDto.Html.ShouldBe(htmlPart.Html);
            partDto.VariableValue.ShouldBe(null);

            var variablePart = (VariableEmailPart)_email.Parts.ElementAt(1);
            partDto = _result.Single(x => x.PartId == variablePart.Id);
            partDto.EmailId.ShouldBe(_email.Id);
            partDto.PartType.ShouldBe(PartType.Variable);
            partDto.Html.ShouldBe(null);
            partDto.VariableValue.ShouldBe(variablePart.Value);

            htmlPart = (HtmlEmailPart)_email.Parts.Last();
            partDto = _result.Single(x => x.PartId == htmlPart.Id);
            partDto.EmailId.ShouldBe(_email.Id);
            partDto.PartType.ShouldBe(PartType.Html);
            partDto.Html.ShouldBe(htmlPart.Html);
            partDto.VariableValue.ShouldBe(null);
        }
    }
}