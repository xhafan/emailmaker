﻿using Core.Commands;
using Core.Domain;
using Core.Utilities.Extensions;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.EmailTemplates;
using EmailMaker.Utilities;

namespace EmailMaker.Commands.Handlers
{
    public class CreateVariableCommandHandler : ICommandMessageHandler<CreateVariableCommand>
    {
        private IRepository<EmailTemplate> _emailTemplateRepository;

        public CreateVariableCommandHandler(IRepository<EmailTemplate> emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public void Execute(CreateVariableCommand command)
        {
            var emailTemplate = _emailTemplateRepository.GetById(command.EmailTemplate.EmailTemplateId);
            command.EmailTemplate.Parts.Each(part =>
                                                 {
                                                     if (part.Html != null)
                                                     {
                                                         emailTemplate.SetHtml(part.PartId, part.Html);
                                                     }
                                                     else if (part.VariableValue != null)
                                                     {
                                                         emailTemplate.SetVariableValue(part.PartId, part.VariableValue);
                                                     }
                                                     else
                                                     {
                                                         throw new EmailMakerException("Unknown email template part, email template id: " + command.EmailTemplate.EmailTemplateId);
                                                     }
                                                 });
            emailTemplate.CreateVariable(command.HtmlTemplatePartId, command.HtmlStartIndex, command.Length);
        }
    }
}
