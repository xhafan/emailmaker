using System.Linq;
using CoreDdd.Nhibernate.Queries;
using CoreDdd.Nhibernate.UnitOfWorks;
using EmailMaker.Domain.Emails;
using EmailMaker.Queries.Messages;
using NHibernate;

namespace EmailMaker.Queries.Handlers
{
    public class GetExistingRecipientsQueryHandler : BaseQueryOverHandler<GetExistingRecipientsQuery>
    {
        public GetExistingRecipientsQueryHandler(NhibernateUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override IQueryOver GetQueryOver<TResult>(GetExistingRecipientsQuery query)
        {
            // todo: implemente XLOCK on the sql and write concurrent locking test for it
            return Session.QueryOver<Recipient>()
                .WhereRestrictionOn(x => x.EmailAddress).IsIn(query.RecipientEmailAddresses.ToArray());
        }
    }
}