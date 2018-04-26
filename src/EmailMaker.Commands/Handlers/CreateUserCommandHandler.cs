using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Domain.Repositories;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.Users;

namespace EmailMaker.Commands.Handlers
{
    public class CreateUserCommandHandler : BaseCommandHandler<CreateUserCommand>
    {
        private readonly IRepository<User> _userRepository;

        public CreateUserCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public override async Task ExecuteAsync(CreateUserCommand command)
        {
            var newUser = new User("", "", command.EmailAddress, command.Password);
            await _userRepository.SaveAsync(newUser);
        }
    }
}