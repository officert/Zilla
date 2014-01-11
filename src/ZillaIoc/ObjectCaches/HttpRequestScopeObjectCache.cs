using System.Web;

namespace ZillaIoc.ObjectCaches
{
    public class HttpRequestScopeObjectCache : IObjectCache
    {
        public TService GetInstance<TService>(Container container, Binding binding)
        {
            var castBinding = (Binding<TService>)binding;

            return (TService)(HttpContext.Current.Items.Contains(binding.Key)
                ? HttpContext.Current.Items[binding.Key]
                : HttpContext.Current.Items[binding.Key] = castBinding.Factory(container));
        }
    }
}