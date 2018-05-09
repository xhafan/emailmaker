using System.Threading.Tasks;
using CoreDdd.Domain.Repositories;
using CoreUtils.Extensions;
using EmailMaker.Domain.Emails;
using EmailMaker.Messages;
using Rebus.Bus;
using Rebus.Handlers;

namespace EmailMaker.Service.Handlers
{
    public class EmailEnqueuedToBeSentEventMessageHandler : IHandleMessages<EmailEnqueuedToBeSentEventMessage>
    {
        private readonly IEmailHtmlBuilder _emailHtmlBuilder;
        private readonly IBus _bus;
        private readonly IRepository<Email> _emailRepository;

        public EmailEnqueuedToBeSentEventMessageHandler(IRepository<Email> emailRepository, IEmailHtmlBuilder emailHtmlBuilder, IBus bus)
        {
            _emailRepository = emailRepository;
            _bus = bus;
            _emailHtmlBuilder = emailHtmlBuilder;
        }

        public async Task Handle(EmailEnqueuedToBeSentEventMessage message)
        {
            var email = await _emailRepository.GetAsync(message.EmailId);
            var emailHtml = _emailHtmlBuilder.BuildHtmlEmail(email.Parts);
            email.EmailRecipients.Each(async x => await _bus.SendLocal(new SendEmailForEmailRecipientMessage
                                                                          {
                                                                              EmailId = email.Id,
                                                                              RecipientId = x.Recipient.Id,
                                                                              FromAddress = email.FromAddress,
                                                                              RecipientEmailAddress = x.Recipient.EmailAddress,
                                                                              RecipientName = x.Recipient.Name,
                                                                              Subject = email.Subject,
                                                                              EmailHtml = emailHtml
                                                                          }));
        }
    }
}
