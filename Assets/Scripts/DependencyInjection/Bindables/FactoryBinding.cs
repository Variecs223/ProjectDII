namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class FactoryBinding<TBase>: IBindable<TBase> 
        where TBase: class
    {
        
        public InjectorContext Context { get; private set; }
        public IFactory<TBase> Factory { get; private set;  }
        
        public FactoryBinding<TBase> Update(ProxyBinding<TBase> proxy, IFactory<TBase> factory)
        {
            Context = proxy.Context;
            Factory = factory;
            return this;
        }
        
        public TBase Inject()
        {
            var instance = Factory.GetInstance();

            if (!Factory.ManuallyInjected)
            {
                Context.Inject(instance);
            }
            
            return instance;
        }

        public bool CheckConditions()
        {
            return true;
        }

        public void Dispose()
        {
            ObjectPool<FactoryBinding<TBase>>.Put(this);
            Factory.Dispose();
        }
    }
}