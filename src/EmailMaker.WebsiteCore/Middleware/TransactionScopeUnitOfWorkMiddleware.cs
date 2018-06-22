using System.Threading.Tasks;
using System.Transactions;
using CoreDdd.UnitOfWorks;
using CoreIoC;
using Microsoft.AspNetCore.Http;
using Rebus.TransactionScopes;

namespace EmailMaker.WebsiteCore.Middleware
{
    // transaction scope is needed to send messages to EmailMaker service only when the DB transaction successfully commits
    // https://stackoverflow.com/a/8169117/379279
    public class TransactionScopeUnitOfWorkMiddleware // todo: extract into a standalone nuget package CoreDdd.AspNetCore? - move Rebus out of this
    {
        private readonly RequestDelegate _next;
        private readonly IsolationLevel _isolationLevel;

        public TransactionScopeUnitOfWorkMiddleware(RequestDelegate next, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _next = next;
            _isolationLevel = isolationLevel;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var transactionScope = _CreateTransactionScope())
            {
                transactionScope.EnlistRebus();

                var unitOfWork = _ResolveUnitOfWorkPerWebRequest();
                unitOfWork.BeginTransaction();

                try
                {
                    await _next.Invoke(context);

                    unitOfWork.Commit();
                    transactionScope.Complete();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
                finally
                {
                    IoC.Release(unitOfWork);
                }
            }
        }

        private IUnitOfWork _ResolveUnitOfWorkPerWebRequest()
        {
            return IoC.Resolve<IUnitOfWork>();
        }

        private TransactionScope _CreateTransactionScope()
        {
            return new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions {IsolationLevel = _isolationLevel},
                TransactionScopeAsyncFlowOption.Enabled
            );
        }
    }
}