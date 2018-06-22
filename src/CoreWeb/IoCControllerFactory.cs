﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using CoreIoC;

namespace CoreWeb
{
    public class IoCControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return base.GetControllerInstance(requestContext, null);
            }

            var controller = (IController)IoC.Resolve(controllerType);
            return controller;
        }

        public override void ReleaseController(IController controller)
        {
            IoC.Release(controller);
            base.ReleaseController(controller);
        }
    }
}
