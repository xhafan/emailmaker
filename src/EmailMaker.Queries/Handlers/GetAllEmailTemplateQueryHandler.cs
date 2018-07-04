using CoreDdd.Nhibernate.Queries;
using CoreDdd.Nhibernate.UnitOfWorks;
using EmailMaker.Dtos.EmailTemplates;
using EmailMaker.Queries.Messages;
using NHibernate;

namespace EmailMaker.Queries.Handlers
{
    public class GetAllEmailTemplateQueryHandler : BaseQueryOverHandler<GetAllEmailTemplateQuery>
    {
        public GetAllEmailTemplateQueryHandler(NhibernateUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {            
        }

        protected override IQueryOver GetQueryOver<TResult>(GetAllEmailTemplateQuery query)
        {
            return Session.QueryOver<EmailTemplateDetailsDto>()
                .Where(e => e.UserId == query.UserId);
        }
    }
}