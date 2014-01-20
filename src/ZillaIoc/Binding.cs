using System;
using ZillaIoc.ObjectCaches;

namespace ZillaIoc
{
    public class Binding
    {
        public string Name { get; set; }
        public Type Service { get; set; }
        public Type Plugin { get; set; }
        public ObjectScope ObjectScope { get; set; }
        public IObjectCache ObjectCache { get; set; }
        public string Key = Guid.NewGuid().ToString();
    }
}