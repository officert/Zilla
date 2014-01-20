using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ZillaIoc.Exceptions;
using ZillaIoc.Extensions;
using ZillaIoc.FluentInterface;
using ZillaIoc.ObjectCaches;
using ZillaIoc.Resources;

namespace ZillaIoc
{
    public class Container : IContainer
    {
        internal readonly ICollection<Binding> Bindings;
        private readonly Dictionary<ObjectScope, IObjectCache> _objectCaches;   //TODO:make thread safe

        public Container()
        {
            Bindings = new Collection<Binding>();

            _objectCaches = new Dictionary<ObjectScope, IObjectCache>
            {
                { ObjectScope.HttpRequest, new HttpRequestScopeObjectCache() } ,
                { ObjectScope.Transient, new TransietScopeObjectCache() },
                { ObjectScope.Singleton, new SingletonScopeObjectCache() }
            };
        }

        #region IContainer

        public void Register(IList<Registry> registries)
        {
            Ensure.ArgumentIsNotNull(registries, "registries");

            foreach (var registry in registries)
            {
                registry.Load();

                if (registry.Bindings != null && registry.Bindings.Any())
                {
                    ProcessAndValidateBindings(registry.Bindings);
                }
            }

            CreateFactoriesForBindings();
        }

        public TService Resolve<TService>()
        {
            return ResolveInternal<TService>();
        }

        public TService Resolve<TService>(Type service)
        {
            var getInstanceMethod = typeof(Container).GetMethod("ResolveInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            var genericMethodInfo = getInstanceMethod.MakeGenericMethod(service);

            return (TService)genericMethodInfo.Invoke(this, null);
        }

        #endregion

        #region Binding Registration

        private void ProcessAndValidateBindings(IEnumerable<Binding> bindings)
        {
            foreach (var binding in bindings)
            {
                Ensure.ArgumentIsNotNull(binding, "binding");
                Ensure.ArgumentIsNotNull(binding.ObjectScope, "binding.ObjectScope");

                var validateBindingMethod = typeof(Container).GetMethod("ValidateBinding", BindingFlags.NonPublic | BindingFlags.Instance);
                var genericMethodInfo = validateBindingMethod.MakeGenericMethod(binding.Service);

                try
                {
                    genericMethodInfo.Invoke(this, new object[] { binding });
                }
                catch (TargetInvocationException ex)
                {
                    if (ex.InnerException != null)
                        throw ex.InnerException;
                }

                Bindings.Add(binding);
            }
        }

        private void ValidateBinding<TService>(Binding binding)
        {
            var castBinding = (Binding<TService>)binding;

            if (castBinding.Factory == null && castBinding.Plugin.IsAnAbstraction())
                throw new InvalidOperationException(string.Format(ExceptionMessages.CannotUseAnAbstractTypeForAPluginType, binding.Plugin, binding.Service));

            CheckIfAnotherBindingAlreadyExistsForTheServiceType(binding);
        }

        private void CreateFactoriesForBindings()
        {
            foreach (var binding in Bindings)
            {
                var registerBindingMethod = typeof(Container).GetMethod("RegisterBinding", BindingFlags.NonPublic | BindingFlags.Instance);
                var genericMethodInfo = registerBindingMethod.MakeGenericMethod(binding.Service);

                genericMethodInfo.Invoke(this, new object[] { binding });
            }
        }

        private void RegisterBinding<TService>(Binding binding)
        {
            var castBinding = (Binding<TService>)binding;

            if (castBinding.Factory == null)
            {
                castBinding.Factory = CreateConstructorFn<TService>(binding.Plugin);
            }
            castBinding.ObjectCache = GetObjectCache(castBinding.ObjectScope);
        }

        private IObjectCache GetObjectCache(ObjectScope objectScope)
        {
            if (!_objectCaches.ContainsKey(objectScope)) throw new InvalidOperationException(string.Format("No object cache exists for the scope '{0}'.", objectScope.ToString()));

            return _objectCaches[objectScope];
        }

        #endregion

        #region Binding Resolution

        private TService ResolveInternal<TService>()
        {
            var binding = FindBinding(typeof(TService));

            if (binding == null)
            {
                //TODO: still need to figure out what we should do when we don't have a binding registered, but the type
                //TODO: they are asking for can be instantiated (ie. not abstract)
                //TODO: options:
                //TODO: 1) create a whole new binding for their service type, with a constructor function, in transient scope,
                //TODO: that way the next time they ask for that service type we already have a factory generated for it
                //TODO: 2) just create the constructor function on the fly each time

                //TODO: currently using option 2

                if (typeof(TService).IsAnAbstraction()) throw new Exception(string.Format("Cannot resolve type '{0}'. The type is an abstract type and no binding was registered for it. To resolve an abstract type you must first register it with the container.", typeof(TService)));

                var factory = CreateConstructorFn<TService>(typeof(TService));

                return factory(this);

                //binding = CreateNewBinding<TService>();

                //_bindings.Add(binding);
            }

            return binding.ObjectCache.GetInstance<TService>(this, binding);
        }

        private Binding FindBinding(Type service)
        {
            return Bindings.FirstOrDefault(x => x.Service == service);
        }

        //private Binding CreateNewBinding<TService>()
        //{
        //    return new Binding<TService>
        //    {
        //        Service = typeof(TService),
        //        Plugin = typeof(TService),
        //        Factory = CreateConstructorFn<TService>(typeof(TService)),
        //        ObjectCache = GetObjectCache(ObjectScope.Transient)
        //    };
        //}

        #endregion

        #region Object Graphing

        private Func<Container, TService> CreateConstructorFn<TService>(Type plugin)
        {
            var ctorExpression = CreateConstructorExp(plugin);

            var containerParamExpression = Expression.Parameter(typeof(Container));

            var lambda = Expression.Lambda<Func<Container, TService>>(ctorExpression, new List<ParameterExpression> { containerParamExpression });

            return lambda.Compile();
        }

        private Expression CreateConstructorExp(Type plugin)
        {
            var ctorInfo = GetConstructorWithMostArgs(plugin);

            var constructorArgs = ctorInfo.GetParameters();

            var constructorArgsExpressions = from arg in constructorArgs
                                             let argBinding = FindBinding(arg.ParameterType)
                                             select CreateResolveExp(argBinding);

            Expression newExp = Expression.New(ctorInfo, constructorArgsExpressions);

            return newExp;
        }

        private Expression CreateResolveExp(Binding binding)
        {
            var resolveMethod = typeof(Container).GetMethod("ResolveInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            var genericMethodInfo = resolveMethod.MakeGenericMethod(binding.Service);

            var containerInstanceExpression = Expression.Constant(this);

            var resolveExp = Expression.Call(containerInstanceExpression, genericMethodInfo);

            return resolveExp;
        }

        private ConstructorInfo GetConstructorWithMostArgs(Type type)
        {
            return type.GetConstructors()
                .OrderByDescending(x => x.GetParameters().Length)
                .FirstOrDefault(ctor => !ctor.IsStatic);
        }

        #endregion

        #region Binding Validation

        /// <summary>
        /// Checks that another binding hasn't already been registered for the same service.
        /// If another binding exists for the same service an exception will be thrown, unless the bindings are named.
        /// If multiple bindings exist for the same service we do not know which is the default binding, so they must be named.
        /// </summary>
        public void CheckIfAnotherBindingAlreadyExistsForTheServiceType(Binding binding)
        {
            var otherBindingsForSameService = Bindings.Where(x => x.Service == binding.Service).ToList();

            if (!otherBindingsForSameService.Any()) return;

            if ((string.IsNullOrEmpty(binding.Name) && otherBindingsForSameService.Any(x => string.IsNullOrEmpty(x.Name))))
                ThrowDetailedBindingConfigurationException(binding, otherBindingsForSameService, ExceptionMessages.CannotHaveMultipleDefaultBindingsForService);

            if (!string.IsNullOrEmpty(binding.Name) && otherBindingsForSameService.Any(x => x.Name == binding.Name))
                ThrowDetailedBindingConfigurationException(binding, otherBindingsForSameService, ExceptionMessages.CannotHaveMultipleNamedBindingsForServiceWithSameName, binding.Name);
        }

        private static void ThrowDetailedBindingConfigurationException(
            Binding binding,
            IEnumerable<Binding> otherBindingsForSameService,
            string errorMessage,
            string bindingName = null)
        {
            const string errorMessageFormatWithNewline = "For<{0}>().Use<{1}>(); {2}";
            const string errorMessageFormatWithNewlineNamed = "For<{0}>().Use<{1}>().Named('{3}'); {2}";
            const string errorMessageFormat = "For<{0}>().Use<{1}>();";
            const string errorMessageFormatNamed = "For<{0}>().Use<{1}>().Named('{2}');";

            var sb = new StringBuilder();

            foreach (var bindingRegistration in otherBindingsForSameService)
            {
                sb.Append(string.IsNullOrEmpty(bindingName)
                    ? string.Format(errorMessageFormatWithNewline, bindingRegistration.Service, bindingRegistration.Plugin, Environment.NewLine)
                    : string.Format(errorMessageFormatWithNewlineNamed, bindingRegistration.Service, bindingRegistration.Plugin, Environment.NewLine, bindingName));
            }
            sb.Append(string.IsNullOrEmpty(bindingName)
                    ? string.Format(errorMessageFormat, binding.Service, binding.Plugin)
                    : string.Format(errorMessageFormatNamed, binding.Service, binding.Plugin, bindingName));

            throw new BindingConfigurationException(string.IsNullOrEmpty(bindingName)
                    ? string.Format(errorMessage, binding.Service, Environment.NewLine, sb)
                    : string.Format(errorMessage, binding.Service, bindingName, Environment.NewLine, sb));
        }

        #endregion
    }
}
