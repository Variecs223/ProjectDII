using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Tiles
{
    public class EmptyTileModel: ITileModel
    {
        public void Dispose()
        {
            ObjectPool<EmptyTileModel>.Put(this);
        }
    }
}