namespace Variecs.ProjectDII.Core.Bindables
{
    public class SingletonBinding<TBase, TSpecific>: IBindable<TBase> 
        where TBase: class 
        where TSpecific: TBase, new()
    {
        public static readonly TSpecific Instance = new TSpecific();
        
        public TBase Inject()
        {
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