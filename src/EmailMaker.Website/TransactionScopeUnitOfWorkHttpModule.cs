using System;
using System.Transactions;
using System.Web;
using CoreDdd.UnitOfWorks;
using CoreIoC;
using CoreUtils.Storages;

namespace EmailMaker.Website
{
    // register TransactionScopeUnitOfWorkHttpModule in the web.config (system.webServer -> modules)
    // transaction scope is needed to send nservicebus messages to EmailMaker service only when the DB transaction successfully commits
    // https://stackoverflow.com/a/8169117/379279
    public class TransactionScopeUnitOfWorkHttpModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
            application.Error += Application_Error;
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            GetTransactionScopePerWebRequest();

            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.BeginTransaction();            
        }

        private void Application_EndRequest(Object source, EventArgs e)
        {
            if (HttpContext.Current.Server.GetLastError() != null) return;

            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.Commit();

            var transactionScope = GetTransactionScopePerWebRequest();
            transactionScope.Complete();
            transactionScope.Dispose();
        }

        private void Application_Error(Object source, EventArgs e)
        {
            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.Rollback();

            var transactionScope = GetTransactionScopePerWebRequest();
            transactionScope.Dispose();
        }

        public void Dispose()
        {
        }

        private IUnitOfWork GetUnitOfWorkPerWebRequest()
        {
            return IoC.Resolve<IUnitOfWork>();
        }

        private TransactionScope GetTransactionScopePerWebRequest()
        {
            var transactionScopeStoragePerWebRequest = IoC.Resolve<IStorage<TransactionScope>>();
            if (transactionScopeStoragePerWebRequest.Get() == null)
            {
                var newTransactionScope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted});
                transactionScopeStoragePerWebRequest.Set(newTransactionScope);
            }
            return transactionScopeStoragePerWebRequest.Get();
        }
    }
}