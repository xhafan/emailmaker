using CoreDdd.Nhibernate.Queries;
using CoreDdd.Nhibernate.UnitOfWorks;
using EmailMaker.Dtos.Users;
using EmailMaker.Queries.Messages;
using NHibernate;

namespace EmailMaker.Queries.Handlers
{
    public class GetUserDetailsByEmailAddressQueryHandler : BaseQueryOverHandler<GetUserDetailsByEmailAddressQuery>
    {
        public GetUserDetailsByEmailAddressQueryHandler(NhibernateUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override IQueryOver GetQueryOver<TResult>(GetUserDetailsByEmailAddressQuery query)
        {
            return Session.QueryOver<UserDto>()
                .Where(x => x.EmailAddress == query.EmailAddress);
        }
    }
}
