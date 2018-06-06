using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailMaker.Service.EmailSenders
{
    public class EmailSender : IEmailSender
    {
        private readonly string _hostname;

        public EmailSender(string hostname)
        {
            _hostname = hostname;
        }

        public async Task SendAsync(MailMessage mailMessage)
        {
            using (var smtpClient = new SmtpClient(_hostname))
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}