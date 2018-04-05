using System;
using System.Web;
using CoreDdd.UnitOfWorks;
using CoreIoC;

namespace EmailMaker.Website
{
    // register UnitOfWorkHttpModule in the web.config (system.webServer -> modules)
    // nservicebus messages are sent to EmailMaker service even when the DB transaction rolls back
    public class UnitOfWorkHttpModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
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