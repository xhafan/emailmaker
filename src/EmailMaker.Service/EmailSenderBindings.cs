using System.Configuration;
using Castle.Core.Smtp;
using Ninject.Modules;

namespace EmailMaker.Service
{
    public class EmailSenderBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IEmailSender>().To<DefaultSmtpSender>()
                .InTransientScope()
                .WithConstructorArgument("hostname", ConfigurationManager.AppSettings["SmtpServer"]);
        }
    }
}