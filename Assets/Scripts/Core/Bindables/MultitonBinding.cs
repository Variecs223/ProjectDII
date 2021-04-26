namespace Variecs.ProjectDII.Core.Bindables
{
    public class MultitonBinding<TBase, TSpecific>: IBindable<TBase> 
        where TBase: class 
        where TSpecific: TBase, new()
    {
        public TBase Inject()
        {
            return new TSpecific();
        }

        public bool CheckConditions()
        {
            return true;
        }

        public void Dispose()
        {
            ObjectPool<MultitonBinding<TBase, TSpecific>>.Put(this);
        }
    }
}