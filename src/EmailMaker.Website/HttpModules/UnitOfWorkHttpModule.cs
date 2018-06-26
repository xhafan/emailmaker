using System;
using System.Data;
using System.Web;
using CoreDdd.Domain.Events;
using CoreDdd.UnitOfWorks;
using CoreIoC;

namespace EmailMaker.Website.HttpModules
{
    // register UnitOfWorkHttpModule in the web.config (system.webServer -> modules)
    public class UnitOfWorkHttpModule : IHttpModule
    {
        private const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;

        public void Init(HttpApplication application)
        {
            DomainEvents.EnableDelayedDomainEventHandling(); // messages sent from domain event handlers would not be sent if the main DB transaction rolls back

            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
            application.Error += Application_Error;
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            var unitOfWork = _ResolveUnitOfWorkPerWebRequest();
            unitOfWork.BeginTransaction(DefaultIsolationLevel);            
        }

        private void Application_EndRequest(Object source, EventArgs e)
        {
            if (HttpContext.Current.Server.GetLastError() != null) return;

            var unitOfWork = _ResolveUnitOfWorkPerWebRequest();
            try
            {
                unitOfWork.Commit();

                DomainEvents.RaiseDelayedEvents(domainEventHandlingAction => _DomainEventHandlingSurroundingTransaction(unitOfWork, domainEventHandlingAction));
            }
            finally
            {
                IoC.Release(unitOfWork);
            }
        }

        private void _DomainEventHandlingSurroundingTransaction(IUnitOfWork unitOfWork, Action domainEventHandlingAction)
        {
            try
            {
                unitOfWork.BeginTransaction(DefaultIsolationLevel);

                domainEventHandlingAction();

                unitOfWork.Commit();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
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
        }

        public void Dispose()
        {
        }

        private IUnitOfWork _ResolveUnitOfWorkPerWebRequest()
        {
            return IoC.Resolve<IUnitOfWork>();
        }
    }
}