using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EmailMaker.Infrastructure;

namespace EmailMaker.Service.EmailSenders
{
    public class EmailSenderInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IEmailSender>()
                    .ImplementedBy<EmailSender>()
                    .DependsOn(new { hostname = AppSettings.Configuration["SmtpServer"] })
                    .LifeStyle.Transient);
        }
    }
}