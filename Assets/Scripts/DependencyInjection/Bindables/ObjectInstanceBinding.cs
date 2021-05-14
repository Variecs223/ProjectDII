namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ObjectInstanceBinding<TBase, TSpecific>: BaseBinding<TBase> 
        where TBase: class 
        where TSpecific: TBase, new()
    {
        public InjectorContext Context { get; private set; }

        public ObjectInstanceBinding<TBase, TSpecific> Update(ProxyBinding<TBase> proxy)
        {
            Context = proxy.Context;
            Conditions = proxy.Conditions;
            return this;
        }
        
        public override TBase Inject()
        {
            var newInstance = new TSpecific();
            
            Context.Inject(newInstance);
            
            return newInstance;
        }

        public override void Dispose()
        {
            ObjectPool<ObjectInstanceBinding<TBase, TSpecific>>.Put(this);
        }
    }
}