namespace Variecs.ProjectDII.DependencyInjection
{
    public class ObjectPoolFactory<TModel>: IFactory<TModel> where TModel: class, new()
    {
        public bool ManuallyInjected => false;
        
        public TModel GetInstance()
        {
            return ObjectPool<TModel>.Get();
        }
        
        public void Dispose()
        {
            ObjectPool<TModel>.Clear();
        }
    }
}