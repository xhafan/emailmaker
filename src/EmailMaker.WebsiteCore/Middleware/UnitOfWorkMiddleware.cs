using System;
using System.Data;
using System.Threading.Tasks;
using CoreDdd.Domain.Events;
using CoreDdd.UnitOfWorks;
using CoreIoC;
using Microsoft.AspNetCore.Http;

namespace EmailMaker.WebsiteCore.Middleware
{
    public class UnitOfWorkMiddleware : IMiddleware
    {
        private readonly IsolationLevel _isolationLevel;

        public UnitOfWorkMiddleware(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _isolationLevel = isolationLevel;
            DomainEvents.EnableDelayedDomainEventHandling(); // make sure messages sent from domain event handlers will not be sent if the main DB transaction rolls back
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                _BeginRequest();

                await next.Invoke(context);

                _EndRequest();
            }
            catch
            {
                _HandleErrorInRequest();
                throw;
            }
        }

        private void _BeginRequest()
        {
            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.BeginTransaction(_isolationLevel);            
        }

        private void _EndRequest()
        {
            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.Commit();

            DomainEvents.RaiseDelayedEvents(_DomainEventHandlingSurroundingTransaction);
        }

        private void _DomainEventHandlingSurroundingTransaction(Action domainEventHandlingAction)
        {
            var unitOfWork = GetUnitOfWorkPerWebRequest();

            try
            {
                unitOfWork.BeginTransaction(_isolationLevel);

                domainEventHandlingAction();

                unitOfWork.Commit();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private void _HandleErrorInRequest()
        {
            var unitOfWork = GetUnitOfWorkPerWebRequest();
            unitOfWork.Rollback();
        }

        private IUnitOfWork GetUnitOfWorkPerWebRequest()
        {
            return IoC.Resolve<IUnitOfWork>();
        }
    }
}