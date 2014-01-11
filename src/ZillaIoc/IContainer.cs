using System;
using System.Collections.Generic;
using ZillaIoc.FluentInterface;

namespace ZillaIoc
{
    public interface IContainer
    {
        void Register(IList<Registry> registries);
        TService Resolve<TService>();
        TService Resolve<TService>(Type service);
    }
}