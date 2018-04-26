using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CoreDdd.Commands;
using CoreDdd.Queries;
using CoreUtils.Extensions;
using EmailMaker.Commands.Messages;
using EmailMaker.Controllers.BaseController;
using EmailMaker.Controllers.ViewModels;
using EmailMaker.Core;
using EmailMaker.Dtos;
using EmailMaker.Dtos.EmailTemplates;
using EmailMaker.Queries.Messages;
using MvcContrib;

namespace EmailMaker.Controllers
{
    public class TemplateController : AuthenticatedController
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IQueryExecutor _queryExecutor;

        public TemplateController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor) : base(queryExecutor)
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        public async Task<ActionResult> Index()
        {
            var emailTemplates = await _queryExecutor.ExecuteAsync<GetAllEmailTemplateQuery, EmailTemplateDetailsDto>(
                new GetAllEmailTemplateQuery{ UserId = await GetUserId() });           
            var model = new TemplateIndexModel { EmailTemplate = emailTemplates };
            return View(model);
        }

        public async Task<ActionResult> Create()
        {            
            var createdEmailTemplateId = default(int);
            var command = new CreateEmailTemplateCommand { UserId = await GetUserId() };
            _commandExecutor.CommandExecuted += args => createdEmailTemplateId = (int) args.Args;
            await _commandExecutor.ExecuteAsync(command);

#pragma warning disable 4014
            return this.RedirectToAction(a => a.Edit(createdEmailTemplateId));
#pragma warning restore 4014
        }

        public async Task<ActionResult> Edit(int id)
        {
            var emailTemplate = await _GetEmailTemplate(id);
            var model = new EmailTemplateEditModel {EmailTemplate = emailTemplate};
            return View(model);
        }

        private async Task<EmailTemplateDto> _GetEmailTemplate(int id)
        {
            var templateMessage = new GetEmailTemplateQuery {EmailTemplateId = id};
            var templatePartMessage = new GetEmailTemplatePartsQuery { EmailTemplateId = id };

            var emailTemplateDtos = await _queryExecutor.ExecuteAsync<GetEmailTemplateQuery, EmailTemplateDto>(templateMessage);
            var emailTemplatePartDtos = await _queryExecutor.ExecuteAsync<GetEmailTemplatePartsQuery, EmailTemplatePartDto>(templatePartMessage);
            
            var emailTemplateDto = emailTemplateDtos.Single();
            emailTemplateDto.Parts = emailTemplatePartDtos;

            return emailTemplateDto;
        }

        [HttpPost]
        public async Task Save(SaveEmailTemplateCommand command)
        {
            await _commandExecutor.ExecuteAsync(command);
        }

        [HttpPost]
        public async Task CreateVariable(CreateVariableCommand command)
        {
            await _commandExecutor.ExecuteAsync(command);
        }

        [HttpPost]
        public async Task DeleteVariable(DeleteVariableCommand command)
        {
            await _commandExecutor.ExecuteAsync(command);
        }

        [HttpPost]
        public async Task<ActionResult> GetEmailTemplate(int id)
        {
            return Json(await _GetEmailTemplate(id));
        }

        public async Task<string> GetHtml(int id)
        {
            var partMessage = new GetEmailTemplatePartsQuery { EmailTemplateId = id };
            var emailTemplatePartDtos = await _queryExecutor.ExecuteAsync<GetEmailTemplatePartsQuery, EmailTemplatePartDto>(partMessage);

            var sb = new StringBuilder();
            emailTemplatePartDtos.Each(part =>
            {
                switch (part.PartType)
                {
                    case PartType.Html:
                        sb.Append(part.Html);
                        break;
                    case PartType.Variable:
                        sb.Append(part.VariableValue);
                        break;
                    default:
                        throw new EmailMakerException("Unknown part type:" + part.PartType);
                }
            });
            return sb.ToString();
        }
    
    }
}
