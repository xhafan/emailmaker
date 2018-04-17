IF OBJECT_ID(N'UserDto') IS NOT NULL
DROP VIEW UserDto
GO

create view UserDto
as
select 
	Id as UserId
	,FirstName
	,LastName
	,EmailAddress
	,[Password]
from [User]
