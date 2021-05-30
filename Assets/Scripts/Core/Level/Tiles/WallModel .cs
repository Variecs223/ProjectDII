using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Tiles
{
    public class WallModel: BaseTileModel
    {
        public override void Dispose()
        {
            base.Dispose();
            
            ObjectPool<WallModel>.Put(this);
        }
    }
}