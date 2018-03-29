using Ninject.Modules;

namespace EmailMaker.Service.Handlers
{
    public class EmailHtmlBuilderBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IEmailHtmlBuilder>().To<EmailHtmlBuilder>().InTransientScope();
        }
    }
}