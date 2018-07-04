using CoreDdd.Nhibernate.Queries;
using CoreDdd.Nhibernate.UnitOfWorks;
using EmailMaker.Dtos.EmailTemplates;
using EmailMaker.Queries.Messages;
using NHibernate;

namespace EmailMaker.Queries.Handlers
{
    public class GetEmailTemplateQueryHandler : BaseQueryOverHandler<GetEmailTemplateQuery>
    {
        public GetEmailTemplateQueryHandler(NhibernateUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override IQueryOver GetQueryOver<TResult>(GetEmailTemplateQuery query)
        {
            return Session.QueryOver<EmailTemplateDto>()
                .Where(e => e.EmailTemplateId == query.EmailTemplateId);
        }
    }
}