using System;

namespace ZillaIoc.FluentInterface
{
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

        public void Use<TPluginType>(TPluginType instance) where TPluginType : TService
        {
            Binding = new Binding<TService>
            {
                Factory = x => instance,
                Plugin = typeof(TPluginType)
            };

            Registry.Register(Binding);
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
