using NUnit.Framework;

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
    }
}
