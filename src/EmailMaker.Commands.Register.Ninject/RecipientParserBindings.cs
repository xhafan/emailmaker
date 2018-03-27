using EmailMaker.Commands.Handlers;
using Ninject.Modules;

namespace EmailMaker.Commands.Register.Ninject
{
    public class RecipientParserBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IRecipientParser>().To<RecipientParser>().InTransientScope();
        }
    }
}