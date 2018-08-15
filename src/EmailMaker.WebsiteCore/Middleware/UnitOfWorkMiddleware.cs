using System;
using System.Data;
using System.Threading.Tasks;
using CoreDdd.Domain.Events;
using CoreDdd.UnitOfWorks;
using Microsoft.AspNetCore.Http;

namespace EmailMaker.WebsiteCore.Middleware
{
    public class UnitOfWorkMiddleware : IMiddleware // todo: extract into a standalone nuget package CoreDdd.AspNetCore?
    {
        private readonly IsolationLevel _isolationLevel;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public UnitOfWorkMiddleware(
            IUnitOfWorkFactory unitOfWorkFactory,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
            )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _isolationLevel = isolationLevel;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var unitOfWork = _unitOfWorkFactory.Create();

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
                _unitOfWorkFactory.Release(unitOfWork);
            }
        }

        private async Task _ExecuteActionInsideUnitOfWork(IUnitOfWork unitOfWork, Func<Task> unitOfWorkAction)
        {
            unitOfWork.BeginTransaction(_isolationLevel);

            try
            {
                await unitOfWorkAction();

                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}