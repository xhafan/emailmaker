drop view if exists "EmailTemplateDetailsDto";

create view "EmailTemplateDetailsDto"
as
select 
	"Id" as "EmailTemplateId",
	"Name",
	"UserId"
from "EmailTemplate"
