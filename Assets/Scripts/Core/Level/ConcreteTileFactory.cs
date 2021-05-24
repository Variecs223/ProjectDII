using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class ConcreteTileFactory<TModel>: IFactory<TModel> where TModel: class, ITileModel, new()
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