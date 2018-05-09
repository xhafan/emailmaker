using CoreDdd.Domain.Events;
using EmailMaker.Domain.Events.Emails;

namespace EmailMaker.UnitTests.Domain.Emails
{
    public class FakeEmailEnqueuedToBeSentDomainEventHandler : IDomainEventHandler<EmailEnqueuedToBeSentDomainEvent>
    {
        public EmailEnqueuedToBeSentDomainEvent RaisedDomainEvent;

        public void Handle(EmailEnqueuedToBeSentDomainEvent domainEvent)
        {
            RaisedDomainEvent = domainEvent;
        }
    }
}