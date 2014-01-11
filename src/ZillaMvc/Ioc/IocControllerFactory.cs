using System;
using System.Web.Mvc;
using System.Web.Routing;
using ZillaIoc;

namespace ZillaMvc.Ioc
{
    public class IocControllerFactory : DefaultControllerFactory
    {
        private readonly Container _container;

        public IocControllerFactory(Container container)
        {
            _container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return _container.Resolve<IController>(controllerType);
        }
    }
}