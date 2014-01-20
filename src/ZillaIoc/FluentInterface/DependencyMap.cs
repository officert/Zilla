using System;

namespace ZillaIoc.FluentInterface
{
    public class DependencyMap
    {
        protected Registry Registry;

        protected Binding Binding;

        //protected string Name { get; set; }

        private ObjectScope _objectScope = ObjectScope.Transient;
        protected ObjectScope ObjectScope
        {
            get
            {
                return _objectScope;
            }
            set
            {
                _objectScope = value;
            }
        }

        protected Type ServiceType { get; set; }
        protected Type PluginType { get; set; }
    }
}