using CoreDdd.UnitOfWorks;
using CoreIoC;
using Rebus.Pipeline;

namespace EmailMaker.Service
{
    public static class RebusUnitOfWork
    {
        public static IUnitOfWork Create(IMessageContext arg)
        {
            var unitOfWork = IoC.Resolve<IUnitOfWork>();
            unitOfWork.BeginTransaction();
            return unitOfWork;
        }

        public static void Commit(IMessageContext messageContext, IUnitOfWork unitOfWork)
        {
            unitOfWork.Commit();
        }

        public static void Rollback(IMessageContext messageContext, IUnitOfWork unitOfWork)
        {
            unitOfWork.Rollback();
        }

        public static void Cleanup(IMessageContext messageContext, IUnitOfWork unitOfWork)
        {
            unitOfWork.Dispose();
        }
    }
}