using System.Net.Mail;
using System.Threading.Tasks;
using EmailMaker.Messages;
using EmailMaker.Service.EmailSenders;
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

        public async Task Handle(SendEmailForEmailRecipientMessage message)
        {
            var fromMailAddress = new MailAddress(message.FromAddress);
            var toMailAddress = new MailAddress(message.RecipientEmailAddress, message.RecipientName);
            var mailMessage = new MailMessage(fromMailAddress, toMailAddress)
                                  {
                                      Subject = message.Subject,
                                      Body = message.EmailHtml,
                                      IsBodyHtml = true
                                  };
            await _emailSender.SendAsync(mailMessage);
        }
    }
}
