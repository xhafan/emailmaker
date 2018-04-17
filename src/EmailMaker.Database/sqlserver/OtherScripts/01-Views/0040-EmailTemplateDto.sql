IF OBJECT_ID(N'EmailTemplateDto') IS NOT NULL
DROP VIEW EmailTemplateDto
GO

create view EmailTemplateDto
as
select 
	Id as EmailTemplateId
	, Name
from EmailTemplate
