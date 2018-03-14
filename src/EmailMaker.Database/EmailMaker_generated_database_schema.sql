
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_EmailRecipient_Email]') AND parent_object_id = OBJECT_ID('[EmailRecipient]'))
alter table [EmailRecipient]  drop constraint FK_EmailRecipient_Email


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_EmailRecipient_Recipient]') AND parent_object_id = OBJECT_ID('[EmailRecipient]'))
alter table [EmailRecipient]  drop constraint FK_EmailRecipient_Recipient


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Email_EmailTemplate]') AND parent_object_id = OBJECT_ID('[Email]'))
alter table [Email]  drop constraint FK_Email_EmailTemplate


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Email_EmailState]') AND parent_object_id = OBJECT_ID('[Email]'))
alter table [Email]  drop constraint FK_Email_EmailState


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKB3CACEE7BB7A8FCD]') AND parent_object_id = OBJECT_ID('[EmailPart]'))
alter table [EmailPart]  drop constraint FKB3CACEE7BB7A8FCD


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC187ECA4EAEB1497]') AND parent_object_id = OBJECT_ID('HtmlEmailPart'))
alter table HtmlEmailPart  drop constraint FKC187ECA4EAEB1497


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK5655592DEAEB1497]') AND parent_object_id = OBJECT_ID('VariableEmailPart'))
alter table VariableEmailPart  drop constraint FK5655592DEAEB1497


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_VariableEmailPart_VariableType]') AND parent_object_id = OBJECT_ID('VariableEmailPart'))
alter table VariableEmailPart  drop constraint FK_VariableEmailPart_VariableType


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC644F053C3BA1A19]') AND parent_object_id = OBJECT_ID('[EmailTemplatePart]'))
alter table [EmailTemplatePart]  drop constraint FKC644F053C3BA1A19


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKE07F3D8E8EBDE1D]') AND parent_object_id = OBJECT_ID('HtmlEmailTemplatePart'))
alter table HtmlEmailTemplatePart  drop constraint FKE07F3D8E8EBDE1D


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK8A7C73968EBDE1D]') AND parent_object_id = OBJECT_ID('RepeatedSectionEmailTemplatePart'))
alter table RepeatedSectionEmailTemplatePart  drop constraint FK8A7C73968EBDE1D


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK2A9FDCE18EBDE1D]') AND parent_object_id = OBJECT_ID('VariableEmailTemplatePart'))
alter table VariableEmailTemplatePart  drop constraint FK2A9FDCE18EBDE1D


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_VariableEmailTemplatePart_VariableType]') AND parent_object_id = OBJECT_ID('VariableEmailTemplatePart'))
alter table VariableEmailTemplatePart  drop constraint FK_VariableEmailTemplatePart_VariableType


    if exists (select * from dbo.sysobjects where id = object_id(N'[EmailRecipient]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [EmailRecipient]

    if exists (select * from dbo.sysobjects where id = object_id(N'[Email]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [Email]

    if exists (select * from dbo.sysobjects where id = object_id(N'[EmailPart]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [EmailPart]

    if exists (select * from dbo.sysobjects where id = object_id(N'HtmlEmailPart') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table HtmlEmailPart

    if exists (select * from dbo.sysobjects where id = object_id(N'VariableEmailPart') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table VariableEmailPart

    if exists (select * from dbo.sysobjects where id = object_id(N'[EmailState]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [EmailState]

    if exists (select * from dbo.sysobjects where id = object_id(N'[Recipient]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [Recipient]

    if exists (select * from dbo.sysobjects where id = object_id(N'[EmailTemplate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [EmailTemplate]

    if exists (select * from dbo.sysobjects where id = object_id(N'[EmailTemplatePart]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [EmailTemplatePart]

    if exists (select * from dbo.sysobjects where id = object_id(N'HtmlEmailTemplatePart') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table HtmlEmailTemplatePart

    if exists (select * from dbo.sysobjects where id = object_id(N'RepeatedSectionEmailTemplatePart') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table RepeatedSectionEmailTemplatePart

    if exists (select * from dbo.sysobjects where id = object_id(N'VariableEmailTemplatePart') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table VariableEmailTemplatePart

    if exists (select * from dbo.sysobjects where id = object_id(N'[VariableType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [VariableType]

    if exists (select * from dbo.sysobjects where id = object_id(N'[User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [User]

    if exists (select * from dbo.sysobjects where id = object_id(N'hibernate_unique_key') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table hibernate_unique_key

    create table [EmailRecipient] (
        Id BIGINT not null,
       Sent BIT not null,
       SentDate DATETIME null,
       EmailId INT not null,
       RecipientId INT not null,
       primary key (Id)
    )

    create table [Email] (
        Id INT not null,
       FromAddress NVARCHAR(MAX) null,
       Subject NVARCHAR(MAX) null,
       EmailTemplateId INT not null,
       EmailStateId INT not null,
       primary key (Id)
    )

    create table [EmailPart] (
        Id INT not null,
       Position INT null,
       EmailId INT null,
       primary key (Id)
    )

    create table HtmlEmailPart (
        EmailPartId INT not null,
       Html NVARCHAR(MAX) not null,
       primary key (EmailPartId)
    )

    create table VariableEmailPart (
        EmailPartId INT not null,
       Value NVARCHAR(MAX) not null,
       VariableTypeId INT not null,
       primary key (EmailPartId)
    )

    create table [EmailState] (
        Id INT not null,
       Name NVARCHAR(255) not null,
       CanSend BIT not null,
       primary key (Id)
    )

    create table [Recipient] (
        Id INT not null,
       EmailAddress NVARCHAR(MAX) not null,
       Name NVARCHAR(MAX) not null,
       primary key (Id)
    )

    create table [EmailTemplate] (
        Id INT not null,
       Name NVARCHAR(MAX) null,
       UserId INT not null,
       primary key (Id)
    )

    create table [EmailTemplatePart] (
        Id INT not null,
       Position INT null,
       EmailTemplateId INT null,
       primary key (Id)
    )

    create table HtmlEmailTemplatePart (
        EmailTemplatePartId INT not null,
       Html NVARCHAR(MAX) not null,
       primary key (EmailTemplatePartId)
    )

    create table RepeatedSectionEmailTemplatePart (
        EmailTemplatePartId INT not null,
       primary key (EmailTemplatePartId)
    )

    create table VariableEmailTemplatePart (
        EmailTemplatePartId INT not null,
       Value NVARCHAR(MAX) not null,
       VariableTypeId INT not null,
       primary key (EmailTemplatePartId)
    )

    create table [VariableType] (
        Id INT not null,
       Name NVARCHAR(MAX) not null,
       primary key (Id)
    )

    create table [User] (
        Id INT not null,
       FirstName NVARCHAR(MAX) not null,
       LastName NVARCHAR(MAX) not null,
       EmailAddress NVARCHAR(MAX) not null,
       Password NVARCHAR(MAX) not null,
       primary key (Id)
    )

    alter table [EmailRecipient] 
        add constraint FK_EmailRecipient_Email 
        foreign key (EmailId) 
        references [Email]

    alter table [EmailRecipient] 
        add constraint FK_EmailRecipient_Recipient 
        foreign key (RecipientId) 
        references [Recipient]

    alter table [Email] 
        add constraint FK_Email_EmailTemplate 
        foreign key (EmailTemplateId) 
        references [EmailTemplate]

    alter table [Email] 
        add constraint FK_Email_EmailState 
        foreign key (EmailStateId) 
        references [EmailState]

    alter table [EmailPart] 
        add constraint FKB3CACEE7BB7A8FCD 
        foreign key (EmailId) 
        references [Email]

    alter table HtmlEmailPart 
        add constraint FKC187ECA4EAEB1497 
        foreign key (EmailPartId) 
        references [EmailPart]

    alter table VariableEmailPart 
        add constraint FK5655592DEAEB1497 
        foreign key (EmailPartId) 
        references [EmailPart]

    alter table VariableEmailPart 
        add constraint FK_VariableEmailPart_VariableType 
        foreign key (VariableTypeId) 
        references [VariableType]

    alter table [EmailTemplatePart] 
        add constraint FKC644F053C3BA1A19 
        foreign key (EmailTemplateId) 
        references [EmailTemplate]

    alter table HtmlEmailTemplatePart 
        add constraint FKE07F3D8E8EBDE1D 
        foreign key (EmailTemplatePartId) 
        references [EmailTemplatePart]

    alter table RepeatedSectionEmailTemplatePart 
        add constraint FK8A7C73968EBDE1D 
        foreign key (EmailTemplatePartId) 
        references [EmailTemplatePart]

    alter table VariableEmailTemplatePart 
        add constraint FK2A9FDCE18EBDE1D 
        foreign key (EmailTemplatePartId) 
        references [EmailTemplatePart]

    alter table VariableEmailTemplatePart 
        add constraint FK_VariableEmailTemplatePart_VariableType 
        foreign key (VariableTypeId) 
        references [VariableType]

    create table hibernate_unique_key (
         next_hi INT 
    )

    insert into hibernate_unique_key values ( 1 )