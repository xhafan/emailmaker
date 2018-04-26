using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Domain.Repositories;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.EmailTemplates;

namespace EmailMaker.Commands.Handlers
{
    public class CreateEmailTemplateCommandHandler : BaseCommandHandler<CreateEmailTemplateCommand>
    {
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;

        public CreateEmailTemplateCommandHandler(IRepository<EmailTemplate> emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public override async Task ExecuteAsync(CreateEmailTemplateCommand command)
        {
            var newEmailTemplate = new EmailTemplate(command.UserId);
            await _emailTemplateRepository.SaveAsync(newEmailTemplate);
            RaiseCommandExecutedEvent(new CommandExecutedArgs { Args = newEmailTemplate.Id });
        }
    }
}