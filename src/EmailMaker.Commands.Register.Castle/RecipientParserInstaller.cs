using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EmailMaker.Commands.Handlers;

namespace EmailMaker.Commands.Register.Castle
{
    public class RecipientParserInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IRecipientParser>()
                    .ImplementedBy<RecipientParser>()
                    .LifeStyle.Transient);
        }
    }
}