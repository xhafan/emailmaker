using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Domain.Repositories;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.Emails;

namespace EmailMaker.Commands.Handlers
{
    public class UpdateEmailVariablesCommandHandler : BaseCommandHandler<UpdateEmailVariablesCommand>
    {
        private readonly IRepository<Email> _emailRepository;

        public UpdateEmailVariablesCommandHandler(IRepository<Email> emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public override async Task ExecuteAsync(UpdateEmailVariablesCommand command)
        {
            var email = await _emailRepository.GetAsync(command.Email.EmailId);
            email.UpdateVariables(command.Email);
        }
    }
}