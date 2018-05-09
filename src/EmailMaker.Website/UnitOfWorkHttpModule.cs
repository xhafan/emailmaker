using System;
using System.Web;
using CoreDdd.Domain.Events;
using CoreDdd.UnitOfWorks;
using CoreIoC;

namespace EmailMaker.Website
{
    // register UnitOfWorkHttpModule in the web.config (system.webServer -> modules)
    public class UnitOfWorkHttpModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            DomainEvents.EnableDelayedDomainEventHandling(); // messages sent from domain event handlers would not be sent if the main DB transaction rolls back

            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
            application.Error += Application_Error;
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.BeginTransaction();            
        }

        private void Application_EndRequest(Object source, EventArgs e)
        {
            if (HttpContext.Current.Server.GetLastError() != null) return;

            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.Commit();

            DomainEvents.RaiseDelayedEvents(_DomainEventHandlingSurroundingTransaction);
        }

        private void _DomainEventHandlingSurroundingTransaction(Action domainEventHandlingAction)
        {
            var unitOfWork = GetUnitOfWorkPerWebRequest();

            try
            {
                unitOfWork.BeginTransaction();

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
            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.Rollback();
        }

        public void Dispose()
        {
        }

        private IUnitOfWork GetUnitOfWorkPerWebRequest()
        {
            return IoC.Resolve<IUnitOfWork>();
        }
    }
}