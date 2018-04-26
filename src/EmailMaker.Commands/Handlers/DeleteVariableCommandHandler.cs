using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Domain.Repositories;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.EmailTemplates;

namespace EmailMaker.Commands.Handlers
{
    public class DeleteVariableCommandHandler : BaseCommandHandler<DeleteVariableCommand>
    {
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;

        public DeleteVariableCommandHandler(IRepository<EmailTemplate> emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public override async Task ExecuteAsync(DeleteVariableCommand command)
        {
            var emailTemplate = await _emailTemplateRepository.GetAsync(command.EmailTemplate.EmailTemplateId);
            emailTemplate.Update(command.EmailTemplate);
            emailTemplate.DeleteVariable(command.VariablePartId);
        }
    }
}