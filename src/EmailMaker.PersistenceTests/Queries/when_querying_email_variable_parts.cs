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
    public class when_querying_email_variable_parts : BasePersistenceTest
    {
        private IEnumerable<EmailPartDto> _result;
        private Email _email;

        [SetUp]
        public void Context()
        {
            _persistEmail();

            var queryHandler = new GetEmailVariablePartsQueryHandler(UnitOfWork);
            _result = queryHandler.Execute<EmailPartDto>(new GetEmailVariablePartsQuery { EmailId = _email.Id });

            void _persistEmail()
            {
                var user = UserBuilder.New.Build();
                Save(user);
                var emailTemplate = EmailTemplateBuilder.New
                    .WithInitialHtml("12345")
                    .WithName("template name")
                    .WithUserId(user.Id)
                    .Build();
                Save(emailTemplate);
                emailTemplate.CreateVariable(emailTemplate.Parts.First().Id, 1, 1);
                Save(emailTemplate);
                emailTemplate.CreateVariable(emailTemplate.Parts.Last().Id, 1, 1);
                Save(emailTemplate);
                var anotherEmailTemplate = EmailTemplateBuilder.New
                    .WithInitialHtml("another html")
                    .WithName("template name")
                    .WithUserId(user.Id)
                    .Build();
                Save(anotherEmailTemplate);

                _email = new Email(emailTemplate);
                Save(_email);
            }
        }

        [Test]
        public void email_template_parts_correctly_retrieved()
        {
            _result.Count().ShouldBe(2);

            var variablePart = (VariableEmailPart)_email.Parts.ElementAt(1);
            var partDto = _result.Single(x => x.PartId == variablePart.Id);
            _VariablePartDtoDataMatchVariableEmailPart(partDto, variablePart);

            variablePart = (VariableEmailPart)_email.Parts.ElementAt(3);
            partDto = _result.Single(x => x.PartId == variablePart.Id);
            _VariablePartDtoDataMatchVariableEmailPart(partDto, variablePart);
        }

        private void _VariablePartDtoDataMatchVariableEmailPart(EmailPartDto partDto, VariableEmailPart variablePart)
        {
            partDto.EmailId.ShouldBe(_email.Id);
            partDto.PartId.ShouldBe(variablePart.Id);
            partDto.PartType.ShouldBe(PartType.Variable);
            partDto.Html.ShouldBe(null);
            partDto.VariableValue.ShouldBe(variablePart.Value);
        }
    }
}