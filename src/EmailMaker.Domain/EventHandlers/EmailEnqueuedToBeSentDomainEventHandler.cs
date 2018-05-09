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

        public void Handle(EmailEnqueuedToBeSentDomainEvent domainEvent) // todo: cannot be async - would throw async used in incorrect place in the request handling pipeline
        {
            var message = new EmailEnqueuedToBeSentEventMessage { EmailId = domainEvent.EmailId };
            _bus.Send(message).Wait();
        }
    }
}
