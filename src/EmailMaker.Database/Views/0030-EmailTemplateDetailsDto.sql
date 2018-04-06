IF OBJECT_ID(N'EmailTemplateDetailsDto') IS NOT NULL
BEGIN
DROP VIEW EmailTemplateDetailsDto
END
GO

create view EmailTemplateDetailsDto
as
select 
	Id as EmailTemplateId,
	Name,
	UserId
from EmailTemplate

go
