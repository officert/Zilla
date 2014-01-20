using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;
using ZillaIoc.FluentInterface;
using ZillaIoc.Resources;

namespace ZillaIoc.Unit.Container
{
    [TestFixture]
    [Category("Unit")]
    public class ContainerTests
    {
        private ZillaIoc.Container _container;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
        }

        [SetUp]
        public void Setup()
        {
            _container = new ZillaIoc.Container();
        }

        #region Register

        [Test]
        public void Register_RegistriesIsNull_ThrowsException()
        {
            //arrange
            List<Registry> registries = null;

            //act + assert
            Assert.That(() => _container.Register(registries),
                Throws.Exception.TypeOf(typeof(ArgumentNullException)).With.Message.EqualTo(string.Format(Ensure.ArgumentIsNullMessageFormat, "registries")));
        }

        #region Registering With Scopes

        [Test]
        public void Register_DependencyMapDoesNotSpecifyScope_TypeIsRegisteredInTransientScope()
        {
            //arrange + act
            _container.Register(new List<Registry>
            {
                new RegistryWithoutScopeSpecified()
            });

            //assert
            foreach (var registration in _container.Bindings)
            {
                registration.ObjectScope.Should().Be.EqualTo(ObjectScope.Transient);
            }
        }

        [Test]
        public void Register_DependencyMapSpecifiesSingletonScope_TypeIsRegisteredInSingletonScope()
        {
            //arrange + act
            _container.Register(new List<Registry>
            {
                new RegistryWithSingletonScopeSpecified()
            });

            //assert
            foreach (var binding in _container.Bindings)
            {
                binding.ObjectScope.Should().Be.EqualTo(ObjectScope.Singleton);
            }
        }

        [Test]
        public void Register_DependencyMapSpecifiesHttpRequestScope_TypeIsRegisteredInHttpRequestScope()
        {
            //arrange + act
            _container.Register(new List<Registry>
            {
                new RegistryWithHttpRequestScopeSpecified()
            });

            //assert
            foreach (var binding in _container.Bindings)
            {
                binding.ObjectScope.Should().Be.EqualTo(ObjectScope.HttpRequest);
            }
        }

        [Test]
        public void Register_DependencyMapSpecifiesTransientScope_TypeIsRegisteredInTransientScope()
        {
            //arrange + act
            _container.Register(new List<Registry>
            {
                new RegistryWithTransientScopeSpecified()
            });

            //assert
            foreach (var binding in _container.Bindings)
            {
                binding.ObjectScope.Should().Be.EqualTo(ObjectScope.Transient);
            }
        }

        #endregion

        #region Registering With Instances

        [Test]
        public void Register_DependencyMapProvidesInstance_TypeIsRegisteredInTransientScope()
        {
            //arrange + act
            _container.Register(new List<Registry>
            {
                new RegistryWithInstance()
            });

            //assert
            foreach (var binding in _container.Bindings)
            {
                binding.ObjectScope.Should().Be.EqualTo(ObjectScope.Transient);
            }
        }

        #endregion


        [Test]
        public void Register_DependencyMapSpecifiesName_TypeIsRegisteredWithName()
        {
            //arrange

            //act
            _container.Register(new List<Registry>
            {
                new RegistryWithNamedMap()
            });

            //assert
            foreach (var binding in _container.Bindings)
            {
                binding.Name.Should().Be.EqualTo("Impl1");
            }
        }

        [Test]
        public void Register_DependencyMapWithInterfaceMappedToInterface_ThrowsException()
        {
            //arrange

            //act + assert
            Assert.That(() => _container.Register(new List<Registry>
            {
                new RegistryWithInterfaceServiceTypeAndInterfacePluginType()
            }),
            Throws.Exception.TypeOf(typeof(InvalidOperationException)).With.Message.EqualTo(string.Format(ExceptionMessages.CannotUseAnAbstractTypeForAPluginType, typeof(ITypeWithDefaultConstructor), typeof(ITypeWithDefaultConstructor))));
        }

        #endregion

        #region Resolve

        [Test]
        public void Resolve_TypeRegisteredWithInstance_ReturnsSameInstance()
        {
            //arrange
            _container.Register(new List<Registry>
            {
                new RegistryWithInstance()
            });

            //act
            var instance1 = _container.Resolve<ITypeWithDefaultConstructor>();
            var instance1PropVal = Guid.NewGuid().ToString();
            instance1.Foobar = instance1PropVal;

            var instance2 = _container.Resolve<ITypeWithDefaultConstructor>();
            var instance2PropVal = Guid.NewGuid().ToString();
            instance2.Foobar = instance2PropVal;

            //assert
            instance1.Should().Be.EqualTo(instance2);
            instance1.Foobar.Should().Be.EqualTo(instance2PropVal);
        }

        #endregion
    }

    #region Registries For Tests

    #region Registering With Scopes

    internal class RegistryWithoutScopeSpecified : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>();
        }
    }

    internal class RegistryWithSingletonScopeSpecified : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>()
                .InSingletonScope();
        }
    }

    internal class RegistryWithHttpRequestScopeSpecified : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>()
                .InHttpRequestScope();
        }
    }

    internal class RegistryWithTransientScopeSpecified : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>()
                .InTransientScope();
        }
    }

    #endregion

    #region Registering With Instances and Factories

    internal class RegistryWithInstance : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use(new TypeWithDefaultConstructor());
        }
    }

    internal class RegistryWithFactory : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use(x => new TypeWithDefaultConstructor()).InSingletonScope();
        }
    }

    #endregion

    internal class RegistryWithNamedMap : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use<TypeWithDefaultConstructor>().Named("Impl1");
        }
    }

    internal class RegistryWithInterfaceServiceTypeAndInterfacePluginType : Registry
    {
        public override void Load()
        {
            For<ITypeWithDefaultConstructor>().Use<ITypeWithDefaultConstructor>();
        }
    }

    #endregion

    /// <summary>
    /// Type with no dependencies.
    /// </summary>
    internal class TypeWithDefaultConstructor : ITypeWithDefaultConstructor
    {
        public string Foobar { get; set; }

        public TypeWithDefaultConstructor()
        {

        }
    }

    internal interface ITypeWithDefaultConstructor
    {
        string Foobar { get; set; }
    }
}
