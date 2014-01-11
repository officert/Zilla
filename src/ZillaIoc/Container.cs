using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ZillaIoc.Extensions;
using ZillaIoc.FluentInterface;
using ZillaIoc.ObjectCaches;

namespace ZillaIoc
{
    public class Container : IContainer
    {
        private readonly ICollection<Binding> _bindings;
        private readonly Dictionary<ObjectScope, IObjectCache> _objectCaches;

        public Container()
        {
            _bindings = new Collection<Binding>();

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

                //Validate bindings before adding them to our internal collection

                _bindings.Add(binding);
            }
        }

        private void CreateFactoriesForBindings()
        {
            foreach (var binding in _bindings)
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

                var factory = CreateConstructorFn<TService>(typeof (TService));

                return factory(this);

                //binding = CreateNewBinding<TService>();

                //_bindings.Add(binding);
            }

            return binding.ObjectCache.GetInstance<TService>(this, binding);
        }

        private Binding FindBinding(Type service)
        {
            return _bindings.FirstOrDefault(x => x.Service == service);
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
    }
}
