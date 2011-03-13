﻿using Core.Queries;
using EmailMaker.DTO.EmailTemplate;
using EmailMaker.Queries.Messages;
using NHibernate;

namespace EmailMaker.Queries.Handlers
{
    public class GetEmailTemplateQuery : BaseNHibernateCriteriaQueryMessageHandler<GetEmailTemplateQueryMessage>
    {
        public override ICriteria GetCriteria<TResult>(GetEmailTemplateQueryMessage message)
        {
            return Session.QueryOver<EmailTemplateDTO>()
                .Where(e => e.EmailTemplateId == message.EmailTemplateId)
                .UnderlyingCriteria;
        }
    }
}