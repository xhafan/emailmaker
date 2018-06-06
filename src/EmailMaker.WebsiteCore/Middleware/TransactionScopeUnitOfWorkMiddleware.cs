using System.Threading.Tasks;
using System.Transactions;
using CoreDdd.UnitOfWorks;
using CoreIoC;
using CoreUtils.Storages;
using Microsoft.AspNetCore.Http;
using Rebus.TransactionScopes;

namespace EmailMaker.WebsiteCore.Middleware
{
    // transaction scope is needed to send messages to EmailMaker service only when the DB transaction successfully commits
    // https://stackoverflow.com/a/8169117/379279
    public class TransactionScopeUnitOfWorkMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _BeginRequest();

            await next.Invoke(context);

            _EndRequest();
        }

        private void _BeginRequest()
        {            
            var transactionScope = GetTransactionScopePerWebRequest();
            transactionScope.EnlistRebus();

            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.BeginTransaction();
        }

        private void _EndRequest()
        {
            //if (HttpContext.Current.Server.GetLastError() != null) return; // todo: check out asp.net core error handling

            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.Commit();

            var transactionScope = GetTransactionScopePerWebRequest();
            transactionScope.Complete();
            transactionScope.Dispose();
        }

        //        private void Application_Error(Object source, EventArgs e) // todo: check out asp.net core error handling
        //        {
        //            var unitOfWork = GetUnitOfWorkPerWebRequest();
        //            unitOfWork.Rollback();
        //
        //            var transactionScope = GetTransactionScopePerWebRequest();
        //            transactionScope.Dispose();
        //        }

        private IUnitOfWork GetUnitOfWorkPerWebRequest()
        {
            return IoC.Resolve<IUnitOfWork>();
        }

        private TransactionScope GetTransactionScopePerWebRequest()
        {
            var transactionScopeStorage = IoC.Resolve<IStorage<TransactionScope>>();
            if (transactionScopeStorage.Get() == null)
            {
                var newTransactionScope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted},
                    TransactionScopeAsyncFlowOption.Enabled
                    );
                transactionScopeStorage.Set(newTransactionScope);
            }
            return transactionScopeStorage.Get();
        }
    }
}