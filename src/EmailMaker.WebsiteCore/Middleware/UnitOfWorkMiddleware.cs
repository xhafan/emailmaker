using System;
using System.Data;
using System.Threading.Tasks;
using CoreDdd.Domain.Events;
using CoreDdd.UnitOfWorks;
using CoreIoC;
using Microsoft.AspNetCore.Http;

namespace EmailMaker.WebsiteCore.Middleware
{
    public class UnitOfWorkMiddleware : IMiddleware // todo: extract into a standalone nuget package CoreDdd.AspNetCore?
    {
        private readonly IsolationLevel _isolationLevel;

        public UnitOfWorkMiddleware(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _isolationLevel = isolationLevel;
            DomainEvents.EnableDelayedDomainEventHandling(); // make sure messages sent from domain event handlers will not be sent if the main DB transaction rolls back
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var unitOfWork = _ResolveUnitOfWorkPerWebRequest();

            try
            {
                await _ExecuteActionInsideUnitOfWork(unitOfWork, () => next.Invoke(context));

                DomainEvents.RaiseDelayedEvents(async domainEventHandlingAction => await _ExecuteActionInsideUnitOfWork(unitOfWork, () =>
                 {
                     domainEventHandlingAction(); // todo: make it really async
                     return Task.CompletedTask; // todo: when async remove this line
                 }));
            }
            finally
            {
                IoC.Release(unitOfWork);
            }
        }

        private async Task _ExecuteActionInsideUnitOfWork(IUnitOfWork unitOfWork, Func<Task> unitOfWorkAction)
        {
            unitOfWork.BeginTransaction(_isolationLevel);

            try
            {
                await unitOfWorkAction();

                unitOfWork.Commit();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private IUnitOfWork _ResolveUnitOfWorkPerWebRequest()
        {
            return IoC.Resolve<IUnitOfWork>();
        }
    }
}