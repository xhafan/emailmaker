using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Domain.Repositories;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.EmailTemplates;

namespace EmailMaker.Commands.Handlers
{
    public class CreateVariableCommandHandler : BaseCommandHandler<CreateVariableCommand>
    {
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;

        public CreateVariableCommandHandler(IRepository<EmailTemplate> emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public override async Task ExecuteAsync(CreateVariableCommand command)
        {
            var emailTemplate = await _emailTemplateRepository.GetAsync(command.EmailTemplate.EmailTemplateId);
            emailTemplate.Update(command.EmailTemplate);
            emailTemplate.CreateVariable(command.HtmlTemplatePartId, command.HtmlStartIndex, command.Length);
        }
    }
}
