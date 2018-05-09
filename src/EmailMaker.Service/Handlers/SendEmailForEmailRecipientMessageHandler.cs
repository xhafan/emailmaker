using System.Net.Mail;
using System.Threading.Tasks;
using Castle.Core.Smtp;
using EmailMaker.Messages;
using Rebus.Handlers;

namespace EmailMaker.Service.Handlers
{
    public class SendEmailForEmailRecipientMessageHandler : IHandleMessages<SendEmailForEmailRecipientMessage>
    {
        private readonly IEmailSender _emailSender;

        public SendEmailForEmailRecipientMessageHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

#pragma warning disable 1998 // async function without await expression
        public async Task Handle(SendEmailForEmailRecipientMessage message)
#pragma warning restore 1998 // async function without await expression
        {
            var fromMailAddress = new MailAddress(message.FromAddress);
            var toMailAddress = new MailAddress(message.RecipientEmailAddress, message.RecipientName);
            var mailMessage = new MailMessage(fromMailAddress, toMailAddress)
                                  {
                                      Subject = message.Subject,
                                      Body = message.EmailHtml,
                                      IsBodyHtml = true
                                  };
            _emailSender.Send(mailMessage);
        }
    }
}
