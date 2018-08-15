using System;
using System.Transactions;
using System.Web;
using CoreDdd.UnitOfWorks;
using CoreIoC;
using CoreUtils.Storages;
using Rebus.TransactionScopes;

namespace EmailMaker.Website.HttpModules
{
    // register TransactionScopeUnitOfWorkHttpModule in the web.config (system.webServer -> modules)
    // transaction scope is needed to send messages to EmailMaker service only when the DB transaction successfully commits
    // https://stackoverflow.com/a/8169117/379279
    public class TransactionScopeUnitOfWorkHttpModule : IHttpModule
    {
        private const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;

        public void Init(HttpApplication application)
        {
            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
            application.Error += Application_Error;
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            var transactionScope = _CreateTransactionScopePerWebRequest();
            transactionScope.EnlistRebus();

            var unitOfWork = _ResolveUnitOfWorkPerWebRequest();
            unitOfWork.BeginTransaction();
        }

        private void Application_EndRequest(Object source, EventArgs e)
        {
            if (HttpContext.Current.Server.GetLastError() != null) return;

            var unitOfWork = _ResolveUnitOfWorkPerWebRequest();
            try
            {
                unitOfWork.Commit();
            }
            finally
            {
                IoC.Release(unitOfWork);
            }

            var transactionScopeStorage = _GetTransactionScopeStoragePerWebRequest();
            var transactionScope = transactionScopeStorage.Get();
            try
            {
                transactionScope.Complete();
                transactionScope.Dispose();
            }
            finally
            {
                IoC.Release(transactionScopeStorage);
            }
        }

        private void Application_Error(Object source, EventArgs e)
        {
            var unitOfWork = _ResolveUnitOfWorkPerWebRequest();
            try
            {
                unitOfWork.Rollback();
            }
            finally
            {
                IoC.Release(unitOfWork);
            }

            var transactionScopeStorage = _GetTransactionScopeStoragePerWebRequest();
            var transactionScope = transactionScopeStorage.Get();
            try
            {
                transactionScope.Dispose();
            }
            finally
            {
                IoC.Release(transactionScopeStorage);
            }
        }

        public void Dispose()
        {
        }

        private IUnitOfWork _ResolveUnitOfWorkPerWebRequest()
        {
            return IoC.Resolve<IUnitOfWork>();
        }

        private TransactionScope _CreateTransactionScopePerWebRequest()
        {
            var transactionScopeStoragePerWebRequest = _GetTransactionScopeStoragePerWebRequest();
            if (transactionScopeStoragePerWebRequest.Get() != null) throw new Exception("Transaction scope already exist");

            var newTransactionScope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions {IsolationLevel = DefaultIsolationLevel},
                    TransactionScopeAsyncFlowOption.Enabled
                    );
            transactionScopeStoragePerWebRequest.Set(newTransactionScope);
            return newTransactionScope;
        }

        private IStorage<TransactionScope> _GetTransactionScopeStoragePerWebRequest()
        {
            return IoC.Resolve<IStorage<TransactionScope>>(); // todo: move this into http context?
        }
    }
}