using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Queries;
using CoreUtils.Extensions;
using EmailMaker.Commands.Messages;
using EmailMaker.Controllers.BaseController;
using EmailMaker.Controllers.ViewModels;
using EmailMaker.Core;
using EmailMaker.Dtos;
using EmailMaker.Dtos.EmailTemplates;
using EmailMaker.Dtos.Emails;
using EmailMaker.Queries.Messages;

#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#endif

#if NETFRAMEWORK 
using System.Web.Mvc;
#endif

namespace EmailMaker.Controllers
{
    public class EmailController : AuthenticatedController
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IQueryExecutor _queryExecutor;

        public EmailController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor) : base(queryExecutor)
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        public async Task<ViewResult> Index()
        {
            var emailTemplates = await _queryExecutor.ExecuteAsync<GetAllEmailTemplateQuery, EmailTemplateDetailsDto>(
                new GetAllEmailTemplateQuery { UserId = await GetUserId()});
            var model = new TemplateIndexModel { EmailTemplate = emailTemplates };
            return View(model);
        }
        
        public async Task<ActionResult> Create(int id)
        {
            var createdEmailId = default(int);
            var command = new CreateEmailCommand {EmailTemplateId = id};
            _commandExecutor.CommandExecuted += args => createdEmailId = (int)args.Args;
            await _commandExecutor.ExecuteAsync(command);

#pragma warning disable 4014
            return RedirectToAction<EmailController>(a => a.EditVariables(createdEmailId));
#pragma warning restore 4014
        }

        public async Task<ActionResult> EditVariables(int id)
        {
            var email = await _GetEmail(id);
            var model = new EmailEditVariablesModel { Email = email };
            return View(model);
        }

        public ActionResult EditRecipients(int id)
        {
            var model = new EmailEditRecipientsModel
                            {
                                EmailId = id,
                                FromAddresses = new[]
                                                    {
                                                        // todo: User.Identity.Name
                                                        "martin@test.com"
                                                    },
                                ToAddresses = new[]
                                                  {
                                                      // todo: User.Identity.Name 
                                                      "martin@test.com"
                                                  },
                                Subject = "subject"
                            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EnqueueEmailToBeSent(
#if NETCOREAPP 
            [FromBody] 
#endif
            EnqueueEmailToBeSentCommand command)
        {
            await _commandExecutor.ExecuteAsync(command);
            return new EmptyResult();
        }

        private async Task<EmailDto> _GetEmail(int id)
        {
            var message = new GetEmailQuery { EmailId = id };
            var variablePartMessage = new GetEmailVariablePartsQuery { EmailId = id };

            var emailDtos = await _queryExecutor.ExecuteAsync<GetEmailQuery, EmailDto>(message);
            var variableEmailPartDtos = await _queryExecutor.ExecuteAsync<GetEmailVariablePartsQuery, EmailPartDto>(variablePartMessage);

            var emailDto = emailDtos.Single();
            emailDto.Parts = variableEmailPartDtos;

            return emailDto;
        }

        [HttpPost]
        public async Task UpdateVariables(
#if NETCOREAPP
            [FromBody] 
#endif            
            UpdateEmailVariablesCommand command)
        {
            await _commandExecutor.ExecuteAsync(command);
        }

        [HttpPost]
        public async Task<ActionResult> GetEmail(int id)
        {
            return Json(await _GetEmail(id));
        }
 
        public async Task<string> GetHtml(int id)
        {
            var partMessage = new GetEmailPartsQuery { EmailId = id };
            var emailPartDtos = await _queryExecutor.ExecuteAsync<GetEmailPartsQuery, EmailPartDto>(partMessage);

            var sb = new StringBuilder();
            emailPartDtos.Each(part =>
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