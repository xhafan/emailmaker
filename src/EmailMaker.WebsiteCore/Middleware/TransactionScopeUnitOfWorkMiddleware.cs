using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;

namespace EmailMaker.WebsiteCore.Middleware
{
    // transaction scope is needed to send messages to EmailMaker service only when the DB transaction successfully commits
    // https://stackoverflow.com/a/8169117/379279
    public class TransactionScopeUnitOfWorkMiddleware : IMiddleware // todo: extract into a standalone nuget package CoreDdd.AspNetCore?
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly Action<TransactionScope> _transactionScopeAfterCreateAction;
        private readonly IsolationLevel _isolationLevel;

        public TransactionScopeUnitOfWorkMiddleware(
            IUnitOfWorkFactory unitOfWorkFactory,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            Action<TransactionScope> transactionScopeAfterCreateAction = null
            )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _isolationLevel = isolationLevel;
            _transactionScopeAfterCreateAction = transactionScopeAfterCreateAction;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using (var transactionScope = _CreateTransactionScope())
            {
                _transactionScopeAfterCreateAction?.Invoke(transactionScope);

                var unitOfWork = _unitOfWorkFactory.Create();
                unitOfWork.BeginTransaction();

                try
                {
                    await next.Invoke(context);

                    await unitOfWork.CommitAsync();
                    transactionScope.Complete();
                }
                catch
                {
                    await unitOfWork.RollbackAsync();
                    throw;
                }
                finally
                {
                    _unitOfWorkFactory.Release(unitOfWork);
                }
            }
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