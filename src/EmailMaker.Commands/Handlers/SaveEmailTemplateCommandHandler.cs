using System.Threading.Tasks;
using CoreDdd.Commands;
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

        public override async Task ExecuteAsync(SaveEmailTemplateCommand command)
        {
            var emailTemplate = await _emailTemplateRepository.GetAsync(command.EmailTemplate.EmailTemplateId);
            emailTemplate.Update(command.EmailTemplate);
        }
    }
}