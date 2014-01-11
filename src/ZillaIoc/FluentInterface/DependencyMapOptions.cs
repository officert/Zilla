namespace ZillaIoc.FluentInterface
{
    public class DependencyMapOptions
    {
        private Binding _binding;
        public DependencyMapOptions(Binding binding)
        {
            _binding = binding;
        }

        public DependencyMapOptions Named(string name)
        {
            _binding.Name = name;
            return this;
        }

        /// <summary>
        /// Default scope. Specifies that the object should not be cached. The container will always return a new instance.
        /// </summary>
        public void InTransientScope()
        {
            _binding.ObjectScope = ObjectScope.Transient;
        }

        /// <summary>
        /// Specifies that the object should be cached as a singleton. The container will return the same instance for the duration of the application.
        /// </summary>
        public void InSingletonScope()
        {
            _binding.ObjectScope = ObjectScope.Singleton;
        }

        /// <summary>
        /// Specifies that the object should be cached per http request. The container will return the same instance for the duration of the http request.
        /// </summary>
        public void InHttpRequestScope()
        {
            _binding.ObjectScope = ObjectScope.HttpRequest;
        }

        /// <summary>
        /// Specifies that the object should be cached per thread. The container will return the same instance for the duration of the thread.
        /// </summary>
        public void InThreadScope()
        {
            _binding.ObjectScope = ObjectScope.ThreadScope;
        }
    }
}