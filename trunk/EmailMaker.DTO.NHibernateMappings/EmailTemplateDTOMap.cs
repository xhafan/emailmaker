﻿using EmailMaker.DTO.EmailTemplate;
using FluentNHibernate.Mapping;

namespace EmailMaker.DTO.NHibernateMappings
{
    public class EmailTemplateDTOMap : ClassMap<EmailTemplateDTO>
    {
        public EmailTemplateDTOMap()
        {
            Table("vw_EmailTemplate");
            Id(x => x.EmailTemplateId);
        }
    }
}


