using CoreDdd.UnitOfWorks;

namespace EmailMaker.WebsiteCore.Middleware
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
        void Release(IUnitOfWork unitOfWork);
    }
}