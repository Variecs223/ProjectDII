using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Tiles
{
    public class WallModel: BaseTileModel
    {
        public override bool AllowObject(ObjectType type)
        {
            return false;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            
            ObjectPool<WallModel>.Put(this);
        }
    }
}