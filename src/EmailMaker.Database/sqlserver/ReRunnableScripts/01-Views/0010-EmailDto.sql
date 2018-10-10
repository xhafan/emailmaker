IF OBJECT_ID(N'EmailDto') IS NOT NULL
DROP VIEW EmailDto
GO

create view EmailDto
as
select 
	Id as EmailId
from Email
