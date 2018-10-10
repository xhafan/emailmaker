IF OBJECT_ID(N'EmailTemplateDetailsDto') IS NOT NULL
DROP VIEW EmailTemplateDetailsDto
GO

create view EmailTemplateDetailsDto
as
select 
	Id as EmailTemplateId,
	Name,
	UserId
from EmailTemplate
