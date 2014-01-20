using ZillaIoc.FluentInterface;
using ZillaMvc.Data;

namespace ZillaMvc.Ioc
{
    public class IocRegistry : Registry
    {
        public override void Load()
        {
            For<IVideoGameRepository>().Use(x => new VideoGameRepository(x.Resolve<IConsoleRepository>())).InSingletonScope();
            For<IConsoleRepository>().Use<ConsoleRepository>();
        }
    }
}