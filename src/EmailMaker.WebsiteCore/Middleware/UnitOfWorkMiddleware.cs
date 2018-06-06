using System;
using System.Threading.Tasks;
using CoreDdd.Domain.Events;
using CoreDdd.UnitOfWorks;
using CoreIoC;
using Microsoft.AspNetCore.Http;

namespace EmailMaker.WebsiteCore.Middleware
{
    public class UnitOfWorkMiddleware : IMiddleware
    {
        public UnitOfWorkMiddleware()
        {
            DomainEvents.EnableDelayedDomainEventHandling(); // make sure messages sent from domain event handlers will not be sent if the main DB transaction rolls back
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _BeginRequest();

            await next.Invoke(context);

            _EndRequest();
        }

        private void _BeginRequest()
        {
            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.BeginTransaction();            
        }

        private void _EndRequest()
        {
            //if (HttpContext.Current.Server.GetLastError() != null) return; // todo: check out asp.net core error handling

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
        //
        //        private void Application_Error(Object source, EventArgs e) // todo: check out asp.net core error handling
        //        {
        //            var unitOfWork = GetUnitOfWorkPerWebRequest();
        //            unitOfWork.Rollback();
        //        }

        private IUnitOfWork GetUnitOfWorkPerWebRequest()
        {
            return IoC.Resolve<IUnitOfWork>();
        }
    }
}