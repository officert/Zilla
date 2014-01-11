namespace ZillaIoc.ObjectCaches
{
    public interface IObjectCache
    {
        TService GetInstance<TService>(Container container, Binding binding);
    }
}
