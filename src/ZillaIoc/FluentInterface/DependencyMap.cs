using System;

namespace ZillaIoc.FluentInterface
{
    public class DependencyMap
    {
        protected Registry Registry;

        protected Binding Binding;

        //protected string Name { get; set; }

        private ObjectScope _objectScope = ObjectScope.Transient;
        protected ObjectScope ObjectScope
        {
            get
            {
                return _objectScope;
            }
            set
            {
                _objectScope = value;
            }
        }

        protected Type ServiceType { get; set; }
        protected Type PluginType { get; set; }
    }

    public class DependencyMap<TService> : DependencyMap
    {
        public Func<Container, TService> Factory { get; set; }

        public DependencyMap(Registry registry)
        {
            Registry = registry;
            ServiceType = typeof(TService);
        }

        public DependencyMapOptions Use<TPluginType>() where TPluginType : TService
        {
            Binding = new Binding<TService> { Plugin = typeof(TPluginType) };

            Registry.Register(Binding);

            return new DependencyMapOptions(Binding);
        }

        public DependencyMapOptions Use<TPluginType>(TPluginType instance) where TPluginType : TService
        {
            Binding = new Binding<TService>
            {
                Factory = x => instance,
                Plugin = typeof(TPluginType)
            };

            Registry.Register(Binding);

            ObjectScope = ObjectScope.Singleton;   //if you provide an instance, the no caching will be used

            return new DependencyMapOptions(Binding);
        }

        public DependencyMapOptions Use(Func<Container, TService> factory)
        {
            Binding = new Binding<TService>
            {
                Factory = factory
            };

            Registry.Register(Binding);

            return new DependencyMapOptions(Binding);
        }
    }
}