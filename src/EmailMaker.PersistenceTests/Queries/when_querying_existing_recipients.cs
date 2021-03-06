using System.Collections.Generic;
using System.Linq;
using CoreDdd.Nhibernate.TestHelpers;
using EmailMaker.Domain.Emails;
using EmailMaker.Queries.Handlers;
using EmailMaker.Queries.Messages;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.PersistenceTests.Queries
{
    [TestFixture]
    public class when_querying_existing_recipients : BasePersistenceTest
    {
        private const string EmailAddressOne = "email1@test.com";
        private const string EmailAddressTwo = "email2@test.com";
        private const string EmailAddressThree = "email3@test.com";
        private IEnumerable<Recipient> _result;
        private Recipient _recipientOne;
        private Recipient _recipientTwo;

        [SetUp]
        public void Context()
        {
            _recipientOne = new Recipient(EmailAddressOne, "name1");
            UnitOfWork.Save(_recipientOne);

            _recipientTwo = new Recipient(EmailAddressTwo, "name2");
            UnitOfWork.Save(_recipientTwo);        

            var queryHandler = new GetExistingRecipientsQueryHandler(UnitOfWork);
            _result = queryHandler.Execute<Recipient>(new GetExistingRecipientsQuery
            {
                RecipientEmailAddresses = new[]
                {
                    EmailAddressOne,
                    EmailAddressTwo,
                    EmailAddressThree
                }
            });
        }

        [Test]
        public void recipients_correctly_retrieved()
        {
            _result.Count().ShouldBe(2);
            _result.First().ShouldBe(_recipientOne);
            _result.Last().ShouldBe(_recipientTwo);
        }
    }
}