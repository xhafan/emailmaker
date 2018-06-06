using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailMaker.Messages;
using EmailMaker.Service.EmailSenders;
using EmailMaker.Service.Handlers;
using FakeItEasy;
using NUnit.Framework;

namespace EmailMaker.Service.Tests.Handlers
{
    [TestFixture]
    public class when_handling_send_email_for_email_recipient_message
    {
        private const int EmailId = 23;
        private const string EmailHtml = "email html";
        private const string FromAddress = "\"John Smith\" <fromAddress@test.com>";
        private const string Subject = "subject";
        private const int RecipientId = 56;
        private const string RecipientEmailAddress = "recipient@test.com";
        private const string RecipientName = "name one";
        private IEmailSender _emailSender;

        [SetUp]
        public async Task Context()
        {
            _emailSender = A.Fake<IEmailSender>();
            var handler = new SendEmailForEmailRecipientMessageHandler(_emailSender);
            await handler.Handle(new SendEmailForEmailRecipientMessage
            {
                EmailId = EmailId,
                RecipientId = RecipientId,
                EmailHtml = EmailHtml,
                FromAddress = FromAddress,
                RecipientEmailAddress = RecipientEmailAddress,
                RecipientName = RecipientName,
                Subject = Subject
            });

        }

        [Test]
        public void email_was_sent()
        {
            A.CallTo(() => _emailSender.SendAsync(A<MailMessage>.That.Matches(p => _MatchMailMessage(p)))).MustHaveHappened();        
        }

        private bool _MatchMailMessage(MailMessage p)
        {
            return p.From.ToString() == FromAddress
                   && p.To.First().ToString() == "\"" + RecipientName + "\" <" + RecipientEmailAddress + ">"
                   && p.Subject == Subject
                   && p.IsBodyHtml
                   && p.Body == EmailHtml;
        }
    }
}