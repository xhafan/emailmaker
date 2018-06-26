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
    public class TransactionScopeUnitOfWorkMiddleware : IMiddleware // todo: extract into a standalone nuget package CoreDdd.AspNetCore? - move Rebus out of this
    {
        private readonly IsolationLevel _isolationLevel;

        public TransactionScopeUnitOfWorkMiddleware(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _isolationLevel = isolationLevel;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using (var transactionScope = _CreateTransactionScope())
            {
                transactionScope.EnlistRebus();

                var unitOfWork = _ResolveUnitOfWorkPerWebRequest();
                unitOfWork.BeginTransaction();

                try
                {
                    await next.Invoke(context);

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
                new TransactionOptions { IsolationLevel = _isolationLevel },
                TransactionScopeAsyncFlowOption.Enabled
            );
        }
    }
}