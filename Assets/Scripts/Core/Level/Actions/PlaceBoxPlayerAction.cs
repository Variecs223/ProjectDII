using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Actions
{
    public class PlaceBoxPlayerAction: IPlayerAction
    {
        [Inject] private LevelModel levelModel;
        
        public bool Perform(Vector2Int coords)
        {
            if (!levelModel.Tiles[coords.y * levelModel.Data.fieldSize.x + coords.x].AllowObject(ObjectType.Box))
            {
                return false;
            }
            
            levelModel.AddObject(ObjectType.Box, coords);
            return true;

        }

        public void Dispose()
        {
            levelModel.Data.UnmarkAsInjected(this);
            levelModel = null;
            ObjectPool<PlaceBoxPlayerAction>.Put(this);
        }
    }
}