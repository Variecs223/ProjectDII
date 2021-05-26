using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Tiles
{
    public class WallModel: ITileModel
    {
        public void Dispose()
        {
            ObjectPool<WallModel>.Put(this);
        }
    }
}