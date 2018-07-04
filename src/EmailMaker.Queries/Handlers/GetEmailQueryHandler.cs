using CoreDdd.Nhibernate.Queries;
using CoreDdd.Nhibernate.UnitOfWorks;
using EmailMaker.Dtos.Emails;
using EmailMaker.Queries.Messages;
using NHibernate;

namespace EmailMaker.Queries.Handlers
{
    public class GetEmailQueryHandler : BaseQueryOverHandler<GetEmailQuery>
    {
        public GetEmailQueryHandler(NhibernateUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override IQueryOver GetQueryOver<TResult>(GetEmailQuery query)
        {
            return Session.QueryOver<EmailDto>()
                .Where(e => e.EmailId == query.EmailId);
        }
    }
}