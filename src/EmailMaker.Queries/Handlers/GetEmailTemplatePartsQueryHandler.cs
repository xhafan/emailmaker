using CoreDdd.Nhibernate.Queries;
using CoreDdd.Nhibernate.UnitOfWorks;
using EmailMaker.Dtos.EmailTemplates;
using EmailMaker.Queries.Messages;
using NHibernate;

namespace EmailMaker.Queries.Handlers
{
    public class GetEmailTemplatePartsQueryHandler : BaseQueryOverHandler<GetEmailTemplatePartsQuery>
    {
        public GetEmailTemplatePartsQueryHandler(NhibernateUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override IQueryOver GetQueryOver<TResult>(GetEmailTemplatePartsQuery query)
        {
            return Session.QueryOver<EmailTemplatePartDto>()
                .Where(e => e.EmailTemplateId == query.EmailTemplateId)
                .OrderBy(x => x.PartId).Asc;
        }
    }
}