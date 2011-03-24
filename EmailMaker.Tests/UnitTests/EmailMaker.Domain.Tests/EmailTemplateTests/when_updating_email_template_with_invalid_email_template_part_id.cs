﻿using System;
using EmailMaker.DTO;
using EmailMaker.DTO.EmailTemplates;
using EmailMaker.TestHelper.Builders;
using NUnit.Framework;

namespace EmailMaker.Domain.Tests.EmailTemplateTests
{
    [TestFixture]
    public class when_updating_email_template_with_invalid_email_template_part_id
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Context()
        {
            var template = EmailTemplateBuilder.New
                .WithId(45)
                .WithInitialHtml("12345")
                .Build();
            var emailTemplateDTO = new EmailTemplateDTO
                                       {
                                           EmailTemplateId = 45,
                                           Parts = new[] { new EmailTemplatePartDTO { PartType = PartType.Html, PartId = 567}}
                                       };
            template.Update(emailTemplateDTO);
        }
    }
}