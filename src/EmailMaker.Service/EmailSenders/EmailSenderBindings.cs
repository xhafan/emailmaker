using EmailMaker.Infrastructure;
using Ninject.Modules;

namespace EmailMaker.Service.EmailSenders
{
    public class EmailSenderBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IEmailSender>().To<EmailSender>()
                .InTransientScope()
                .WithConstructorArgument("hostname", AppSettings.Configuration["SmtpServer"]);
        }
    }
}