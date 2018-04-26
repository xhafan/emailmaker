using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CoreDdd.Queries;
using EmailMaker.Dtos.Users;
using EmailMaker.Queries.Messages;

namespace EmailMaker.Controllers.BaseController
{
    [Authorize]
    public class AuthenticatedController : Controller
    {
        private readonly IQueryExecutor _queryExecutor;

        public AuthenticatedController(IQueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        protected async Task<int> GetUserId()
        {
            // todo: fix this user id retrieval via cookie persistence or in identity
            var message = new GetUserDetailsByEmailAddressQuery {EmailAddress = User.Identity.Name};
            var user = (await _queryExecutor.ExecuteAsync<GetUserDetailsByEmailAddressQuery, UserDto>(message)).Single();
            return user.UserId;
        }
    }
}
