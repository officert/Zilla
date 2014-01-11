using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ZillaIoc.FluentInterface
{
    public abstract class Registry
    {
        internal ICollection<Binding> Bindings = new Collection<Binding>();

        public virtual void Load()
        {
        }

        internal void Register(Binding binding)
        {
            Ensure.ArgumentIsNotNull(binding, "binding");

            Bindings.Add(binding);
        }

        public DependencyMap<TServiceType> For<TServiceType>()
        {
            return new DependencyMap<TServiceType>(this);
        }
    }
}