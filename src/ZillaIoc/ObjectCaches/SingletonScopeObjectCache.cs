using System.Collections.Generic;

namespace ZillaIoc.ObjectCaches
{
    public class SingletonScopeObjectCache : IObjectCache
    {
        private readonly Dictionary<string, object> _instances = new Dictionary<string, object>();

        public TService GetInstance<TService>(Container container, Binding binding)
        {
            var castBinding = (Binding<TService>)binding;

            return (TService)(_instances.ContainsKey(binding.Key)
                ? _instances[binding.Key]
                : _instances[binding.Key] = castBinding.Factory(container));
        }
    }
}