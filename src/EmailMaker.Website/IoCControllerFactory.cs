using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;

namespace EmailMaker.Website
{
    public class IoCControllerFactory : DefaultControllerFactory
    {
        private readonly IWindsorContainer _iocContainer;

        public IoCControllerFactory(IWindsorContainer iocContainer)
        {
            _iocContainer = iocContainer;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return base.GetControllerInstance(requestContext, null);
            }

            var controller = (IController)_iocContainer.Resolve(controllerType);
            return controller;
        }

        public override void ReleaseController(IController controller)
        {
            _iocContainer.Release(controller);
            base.ReleaseController(controller);
        }
    }
}
