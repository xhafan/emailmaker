﻿using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Domain.Repositories;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.Emails;
using EmailMaker.Domain.EmailTemplates;

namespace EmailMaker.Commands.Handlers
{
    public class CreateEmailCommandHandler : BaseCommandHandler<CreateEmailCommand>
    {
        private readonly IRepository<Email> _emailRepository;
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;

        public CreateEmailCommandHandler(IRepository<Email> emailRepository, IRepository<EmailTemplate> emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _emailRepository = emailRepository;
        }

        public override async Task ExecuteAsync(CreateEmailCommand command)
        {
            var emailTemplate = await _emailTemplateRepository.GetAsync(command.EmailTemplateId);
            var newEmail = new Email(emailTemplate);
            await _emailRepository.SaveAsync(newEmail);
            RaiseCommandExecutedEvent(new CommandExecutedArgs { Args = newEmail.Id });
        }
    }
}