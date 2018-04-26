using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Domain.Repositories;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.Users;

namespace EmailMaker.Commands.Handlers
{
    public class ChangePasswordForUserCommandHandler : BaseCommandHandler<ChangePasswordForUserCommand>
    {
        private readonly IRepository<User> _userRepository;

        public ChangePasswordForUserCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public override async Task ExecuteAsync(ChangePasswordForUserCommand command)
        {
            var user = await _userRepository.GetAsync(command.UserId);
            user.ChangePassword(command.OldPassword, command.NewPassword);
        }
    }
}