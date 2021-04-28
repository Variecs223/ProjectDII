namespace Variecs.ProjectDII.Core.Bindables
{
    public class SingletonBinding<TBase, TSpecific>: IBindable<TBase> 
        where TBase: class 
        where TSpecific: TBase, new()
    {
        public static readonly TSpecific Instance = new TSpecific();
        // ReSharper disable once StaticMemberInGenericType
        private static bool instanceInjected;
        
        public TBase Inject()
        {
            if (instanceInjected)
            {
                return Instance;
            }
            
            InjectorContext.BaseContext.Inject(Instance);
            instanceInjected = true;
            
            return Instance;
        }

        public bool CheckConditions()
        {
            return true;
        }

        public void Dispose()
        {
            ObjectPool<SingletonBinding<TBase, TSpecific>>.Put(this);
        }
    }
}