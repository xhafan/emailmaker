using System;
using Castle.Windsor;

namespace CoreIoC
{
    public static class IoC // todo: make this lib indepedent on Castle
    {
        private static IWindsorContainer _container;

        public static void Initialize(IWindsorContainer container)
        {
            _container = container;
        }

        public static IWindsorContainer Container
        {
            get
            {
                if (_container == null)
                {
                    throw new InvalidOperationException("The container has not been initialized! Please call IoC.Initialize(container) before using it.");
                }
                return _container;
            }
        }
        
        public static object Resolve(Type service)
        {
            return Container.Resolve(service);
        }

        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static T[] ResolveAll<T>()
        {
            return Container.ResolveAll<T>();
        }

        public static void Release(object instance)
        {
            Container.Release(instance);
        }
    
    }
}
