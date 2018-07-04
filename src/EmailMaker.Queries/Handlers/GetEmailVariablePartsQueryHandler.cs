using CoreDdd.Nhibernate.Queries;
using CoreDdd.Nhibernate.UnitOfWorks;
using EmailMaker.Dtos;
using EmailMaker.Dtos.Emails;
using EmailMaker.Queries.Messages;
using NHibernate;

namespace EmailMaker.Queries.Handlers
{
    public class GetEmailVariablePartsQueryHandler : BaseQueryOverHandler<GetEmailVariablePartsQuery>
    {
        public GetEmailVariablePartsQueryHandler(NhibernateUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override IQueryOver GetQueryOver<TResult>(GetEmailVariablePartsQuery query)
        {
            return Session.QueryOver<EmailPartDto>()
                .Where(e => e.EmailId == query.EmailId && e.PartType == PartType.Variable);
        }
    }
}