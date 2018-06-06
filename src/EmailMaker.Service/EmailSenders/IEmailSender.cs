using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailMaker.Service.EmailSenders
{
    public interface IEmailSender
    {
        Task SendAsync(MailMessage mailMessage);
    }
}