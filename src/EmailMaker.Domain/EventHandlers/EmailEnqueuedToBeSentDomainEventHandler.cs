using CoreDdd.Domain.Events;
using EmailMaker.Domain.Events.Emails;
using EmailMaker.Messages;
using Rebus.Bus;

namespace EmailMaker.Domain.EventHandlers
{
    public class EmailEnqueuedToBeSentDomainEventHandler : IDomainEventHandler<EmailEnqueuedToBeSentDomainEvent>
    {
        private readonly IBus _bus;

        public EmailEnqueuedToBeSentDomainEventHandler(IBus bus)
        {
            _bus = bus;
        }

        public void Handle(EmailEnqueuedToBeSentDomainEvent domainEvent)
        {
            var message = new EmailEnqueuedToBeSentEventMessage { EmailId = domainEvent.EmailId };
            _bus.Publish(message).Wait();
        }
    }
}
