﻿using System.Linq;
using CoreDdd.Nhibernate.TestHelpers;
using EmailMaker.Domain.EmailTemplates;
using EmailMaker.TestHelper.Builders;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.PersistenceTests.Domain.EmailTemplates
{
    [TestFixture]
    public class when_persisting_email_template_while_creating_and_deleting_variables : BasePersistenceTest
    {
        private EmailTemplate _emailTemplate;
        private EmailTemplate _retrievedEmailTemplate;

        [Test]
        public void test_multiple_variable_creation_and_deletion_persistence()
        {
            var user = UserBuilder.New.Build();
            Save(user);
            _emailTemplate = EmailTemplateBuilder.New
                .WithInitialHtml("12345")
                .WithUserId(user.Id)
                .Build();
            Save(_emailTemplate);            
            Clear();
            _retrievedEmailTemplate = Get<EmailTemplate>(_emailTemplate.Id);
            _CheckThatRetrievedEmailTemplateIsTheSameAsEmailTemplate();
            
            _emailTemplate = _retrievedEmailTemplate;
            _emailTemplate.CreateVariable(_emailTemplate.Parts.First().Id, 1, 1);
            Save(_emailTemplate);
            Clear();
            _retrievedEmailTemplate = Get<EmailTemplate>(_emailTemplate.Id);
            _CheckThatRetrievedEmailTemplateIsTheSameAsEmailTemplate();

            _emailTemplate = _retrievedEmailTemplate;
            _emailTemplate.CreateVariable(_emailTemplate.Parts.Last().Id, 1, 1);
            Save(_emailTemplate);
            Clear();
            _retrievedEmailTemplate = Get<EmailTemplate>(_emailTemplate.Id);
            _CheckThatRetrievedEmailTemplateIsTheSameAsEmailTemplate();

            _emailTemplate = _retrievedEmailTemplate;
            _emailTemplate.DeleteVariable(_emailTemplate.Parts.ElementAt(1).Id);
            Save(_emailTemplate);
            Clear();
            _retrievedEmailTemplate = Get<EmailTemplate>(_emailTemplate.Id);
            _CheckThatRetrievedEmailTemplateIsTheSameAsEmailTemplate();        
        }

        private void _CheckThatRetrievedEmailTemplateIsTheSameAsEmailTemplate()
        {
            _retrievedEmailTemplate.Id.ShouldBe(_emailTemplate.Id);
            _retrievedEmailTemplate.Parts.Count().ShouldBe(_emailTemplate.Parts.Count());
            var position = 0;
            foreach (var retrievedPart in _retrievedEmailTemplate.Parts)
            {
                var part = _emailTemplate.Parts.First(x => x.Id == retrievedPart.Id);
                retrievedPart.Position.ShouldBe(position++);

                switch (retrievedPart)
                {
                    case HtmlEmailTemplatePart htmlRetrievedPart:
                        var htmlPart = (HtmlEmailTemplatePart)part;
                        htmlRetrievedPart.Html.ShouldBe(htmlPart.Html);
                        break;
                    case VariableEmailTemplatePart variableRetrievedPart:
                        var variablePart = (VariableEmailTemplatePart)part;
                        variableRetrievedPart.Value.ShouldBe(variablePart.Value);
                        break;
                }                
            }
        }
    }
}