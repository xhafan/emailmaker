drop view if exists EmailTemplateDto;

create view EmailTemplateDto
as
select 
	Id as EmailTemplateId
	, Name
from EmailTemplate
