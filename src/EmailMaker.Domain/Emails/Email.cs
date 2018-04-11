﻿using System.Collections.Generic;
using System.Linq;
using CoreDdd.Domain;
using CoreDdd.Domain.Events;
using CoreUtils;
using CoreUtils.Extensions;
using EmailMaker.Core;
using EmailMaker.Domain.Emails.EmailStates;
using EmailMaker.Domain.EmailTemplates;
using EmailMaker.Domain.Events.Emails;
using EmailMaker.Dtos;
using EmailMaker.Dtos.Emails;

namespace EmailMaker.Domain.Emails
{
    public class Email : Entity, IAggregateRoot
    {
        private readonly IList<EmailPart> _parts = new List<EmailPart>();
        private readonly ICollection<EmailRecipient> _emailRecipients = new HashSet<EmailRecipient>();

        protected Email() {}

        public Email(EmailTemplate emailTemplate)
        {
            EmailTemplate = emailTemplate;
            State = EmailState.Draft;

            foreach (var emailTemplatePart in emailTemplate.Parts)
            {
                switch (emailTemplatePart)
                {
                    case HtmlEmailTemplatePart htmlEmailTemplatePart:
                        _parts.Add(new HtmlEmailPart(htmlEmailTemplatePart.Html));
                        break;
                    case VariableEmailTemplatePart variableEmailTemplatePart:
                        _parts.Add(new VariableEmailPart(variableEmailTemplatePart.VariableType, variableEmailTemplatePart.Value));
                        break;
                    default:
                        throw new EmailMakerException("Unsupported email template part: " + emailTemplatePart.GetType());
                }
            }           
        }

        public virtual EmailTemplate EmailTemplate { get; protected set; }
        public virtual string FromAddress { get; protected set; }
        public virtual string Subject { get; protected set; }
        public virtual EmailState State { get; protected set; }
        public virtual IEnumerable<EmailPart> Parts => _parts;
        public virtual IEnumerable<EmailRecipient> EmailRecipients => _emailRecipients;

        public virtual void UpdateVariables(EmailDto emailDto)
        {
            Guard.Hope(Id == emailDto.EmailId, "Invalid email id");
            emailDto.Parts.Each(part =>
            {
                if (part.PartType == PartType.Variable)
                {
                    _SetVariableValue(part.PartId, part.VariableValue);
                }
                else
                {
                    throw new EmailMakerException("Unknown email part type: " + part.PartType);
                }
            });
        }

        private VariableEmailPart _GetVariablePart(int variablePartId)
        {
            return (VariableEmailPart)_GetPart(variablePartId);
        }

        private EmailPart _GetPart(int partId)
        {
            return _parts.First(x => x.Id == partId);
        }

        private void _SetVariableValue(int variablePartId, string value)
        {
            _GetVariablePart(variablePartId).SetValue(value);
        }

        public virtual void EnqueueEmailToBeSent(string fromAddress, HashSet<Recipient> recipients, string subject)
        {
            Guard.Hope(State.CanSend, "cannot enqeue email in the current state: " + State.Name);
            Guard.Hope(_emailRecipients.Count == 0, "recipients must be empty");
            State = EmailState.ToBeSent;
            FromAddress = fromAddress;
            Subject = subject;

            recipients.Each(r => _emailRecipients.Add(new EmailRecipient(this, r)));

            DomainEvents.RaiseEvent(new EmailEnqueuedToBeSentDomainEvent
                                        {
                                            EmailId = Id
                                        });
        }
    }
}
