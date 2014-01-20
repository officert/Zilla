using System;

namespace ZillaIoc
{
    public class Binding<TService> : Binding
    {
        internal Func<Container, TService> Factory { get; set; }

        public Binding()
        {
            Service = typeof(TService);
        }
    }
}
