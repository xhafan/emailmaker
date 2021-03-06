﻿using System;
using System.Linq;
using EmailMaker.TestHelper.Builders;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.UnitTests.Domain.EmailTemplates
{
    [TestFixture]
    public class when_creating_variable_with_incorrect_string_index
    {
        private ArgumentOutOfRangeException _exception;

        [SetUp]
        public void Context()
        {
            var emailTemplate = EmailTemplateBuilder.New
                .WithInitialHtml("html")
                .Build();
            var htmlTeplatePartId = emailTemplate.Parts.First().Id;
            _exception = Should.Throw<ArgumentOutOfRangeException>(() => emailTemplate.CreateVariable(htmlTeplatePartId, -1, 0));
        }

        [Test]
        public void exception_was_thrown()
        {
            _exception.Message.ToLower().ShouldMatch("length cannot be less than zero");
        }
    }
}