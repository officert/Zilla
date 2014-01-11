namespace ZillaIoc.ObjectCaches
{
    public class TransietScopeObjectCache : IObjectCache
    {
        public TService GetInstance<TService>(Container container, Binding binding)
        {
            var castBinding = (Binding<TService>)binding;

            return castBinding.Factory(container);
        }
    }
}