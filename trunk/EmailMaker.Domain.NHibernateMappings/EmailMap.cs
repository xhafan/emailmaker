﻿using EmailMaker.Domain.Emails;
using FluentNHibernate.Mapping;

namespace EmailMaker.Domain.NHibernateMappings
{
    public class EmailMap : ClassMap<Email>
    {
        public EmailMap()
        {
            Id(x => x.Id)
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .GeneratedBy.HiLo("100");

            References(x => x.EmailTemplate, "EmailTemplateId");
            HasMany(x => x.Parts)
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .KeyColumn("EmailId")
                .AsList(a => a.Column("Position"))
                .Cascade.AllDeleteOrphan();
        }
    }
}