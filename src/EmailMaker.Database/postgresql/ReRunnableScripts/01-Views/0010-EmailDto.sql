drop view if exists "EmailDto";

create view "EmailDto"
as
select 
	"Id" as "EmailId"
from "Email"
