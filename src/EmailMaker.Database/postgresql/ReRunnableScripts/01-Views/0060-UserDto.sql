drop view if exists "UserDto";

create view "UserDto"
as
select 
	"Id" as "UserId"
	,"FirstName"
	,"LastName"
	,"EmailAddress"
	,"Password"
from "User"
