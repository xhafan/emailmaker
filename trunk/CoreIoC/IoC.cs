using System;

namespace CoreIoC
{
    public static class IoC
    {
        private static IContainer _container;

        public static void Initialize(IContainer container)
        {
            _container = container;
        }

        private static IContainer Container
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
