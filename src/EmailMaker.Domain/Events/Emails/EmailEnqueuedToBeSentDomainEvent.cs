using CoreDdd.Domain.Events;

namespace EmailMaker.Domain.Events.Emails
{
    public class EmailEnqueuedToBeSentDomainEvent : IDomainEvent
    {
        public int EmailId { get; set; }
    }
}
