using CoreDdd.Nhibernate.Queries;
using EmailMaker.Dtos.EmailTemplates;
using EmailMaker.Queries.Messages;
using NHibernate;

namespace EmailMaker.Queries.Handlers
{
    public class GetAllEmailTemplateQueryHandler : BaseQueryOverHandler<GetAllEmailTemplateQuery>
    {
        protected override IQueryOver GetQueryOver<TResult>(GetAllEmailTemplateQuery query)
        {
            return Session.QueryOver<EmailTemplateDetailsDto>()
                .Where(e => e.UserId == query.UserId);
        }
    }
}