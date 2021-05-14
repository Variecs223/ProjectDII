namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class SingletonBinding<TBase, TSpecific>: BaseBinding<TBase>
        where TBase: class 
        where TSpecific: TBase, new()
    {
        public static readonly TSpecific Instance = new TSpecific();
        // ReSharper disable once StaticMemberInGenericType
        private static bool instanceInjected;

        public InjectorContext Context { get; private set; }

        public SingletonBinding<TBase, TSpecific> Update(ProxyBinding<TBase> proxy)
        {
            Context = proxy.Context;
            Conditions = proxy.Conditions;
            return this;
        }
        
        public override TBase Inject()
        {
            if (instanceInjected)
            {
                return Instance;
            }
            
            Context.Inject(Instance);
            instanceInjected = true;
            
            return Instance;
        }

        public override void Dispose()
        {
            ObjectPool<SingletonBinding<TBase, TSpecific>>.Put(this);
        }
    }
}