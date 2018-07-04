using CoreDdd.Nhibernate.Queries;
using CoreDdd.Nhibernate.UnitOfWorks;
using EmailMaker.Dtos.Emails;
using EmailMaker.Queries.Messages;
using NHibernate;

namespace EmailMaker.Queries.Handlers
{
    public class GetEmailPartsQueryHandler : BaseQueryOverHandler<GetEmailPartsQuery>
    {
        public GetEmailPartsQueryHandler(NhibernateUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override IQueryOver GetQueryOver<TResult>(GetEmailPartsQuery query)
        {
            return Session.QueryOver<EmailPartDto>()
                .Where(e => e.EmailId == query.EmailId);
        }
    }
}