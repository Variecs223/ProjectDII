namespace Variecs.ProjectDII.Core.Bindables
{
    public class ObjectInstanceBinding<TBase, TSpecific>: IBindable<TBase> 
        where TBase: class 
        where TSpecific: TBase, new()
    {
        public InjectorContext Context { get; private set; }

        public ObjectInstanceBinding<TBase, TSpecific> Update(ProxyBinding<TBase> proxy)
        {
            Context = proxy.Context;
            return this;
        }
        
        public TBase Inject()
        {
            var newInstance = new TSpecific();
            
            Context.Inject(newInstance);
            
            return newInstance;
        }

        public bool CheckConditions()
        {
            return true;
        }

        public void Dispose()
        {
            ObjectPool<ObjectInstanceBinding<TBase, TSpecific>>.Put(this);
        }
    }
}