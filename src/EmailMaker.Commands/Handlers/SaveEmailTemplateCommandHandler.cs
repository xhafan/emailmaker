﻿using CoreDdd.Commands;
using CoreDdd.Domain.Repositories;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.EmailTemplates;

namespace EmailMaker.Commands.Handlers
{
    public class SaveEmailTemplateCommandHandler : BaseCommandHandler<SaveEmailTemplateCommand>
    {
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;

        public SaveEmailTemplateCommandHandler(IRepository<EmailTemplate> emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public override void Execute(SaveEmailTemplateCommand command)
        {
            var emailTemplate = _emailTemplateRepository.Get(command.EmailTemplate.EmailTemplateId);
            emailTemplate.Update(command.EmailTemplate);
        }
    }
}