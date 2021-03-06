using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDdd.Commands;
using CoreDdd.Domain.Repositories;
using CoreDdd.Queries;
using CoreUtils.Extensions;
using EmailMaker.Commands.Messages;
using EmailMaker.Domain.Emails;
using EmailMaker.Queries.Messages;

namespace EmailMaker.Commands.Handlers
{
    public class EnqueueEmailToBeSentCommandHandler : BaseCommandHandler<EnqueueEmailToBeSentCommand>
    {
        private readonly IRepository<Email> _emailRepository;
        private readonly IRecipientParser _recipientParser;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IRepository<Recipient> _recipientRepository;

        public EnqueueEmailToBeSentCommandHandler(IRepository<Email> emailRepository, IRecipientParser recipientParser, IQueryExecutor queryExecutor,
            IRepository<Recipient> recipientRepository)
        {
            _recipientRepository = recipientRepository;
            _emailRepository = emailRepository;
            _queryExecutor = queryExecutor;
            _recipientParser = recipientParser;
        }

        public override async Task ExecuteAsync(EnqueueEmailToBeSentCommand command)
        {
            var email = await _emailRepository.GetAsync(command.EmailId);
            var emailAddressesAndNames = _recipientParser.Parse(command.Recipients);
            var existingRecipients = (await _queryExecutor.ExecuteAsync<GetExistingRecipientsQuery, Recipient>(
                new GetExistingRecipientsQuery
                    {
                        RecipientEmailAddresses = emailAddressesAndNames.Keys
                    })).ToDictionary(k => k.EmailAddress);
            var recipients = new HashSet<Recipient>(existingRecipients.Values);
            var recipientsToBeCreated = emailAddressesAndNames.Where(p => !existingRecipients.ContainsKey(p.Key));
            recipientsToBeCreated.Each(async r =>
                                           {
                                               var newRecipient = new Recipient(r.Key, r.Value);
                                               await _recipientRepository.SaveAsync(newRecipient);
                                               recipients.Add(newRecipient);
                                           });
            email.EnqueueEmailToBeSent(command.FromAddress, recipients, command.Subject);
        }
    }
}