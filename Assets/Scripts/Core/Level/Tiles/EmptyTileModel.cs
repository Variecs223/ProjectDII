using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Tiles
{
    public class EmptyTileModel: BaseTileModel
    {
        public override void Dispose()
        {
            base.Dispose();
            
            ObjectPool<EmptyTileModel>.Put(this);
        }
    }
}