using CoreDdd.Nhibernate.Configurations;
using Ninject.Modules;

namespace EmailMaker.Infrastructure.Register.Ninject
{
    public class EmailMakerNhibernateBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<INhibernateConfigurator>().To<EmailMakerNhibernateConfigurator>().InSingletonScope();
        }
    }
}