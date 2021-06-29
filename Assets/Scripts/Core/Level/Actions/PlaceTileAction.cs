using UnityEngine;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Actions
{
    public class PlaceTileAction: IAction
    {
        [Inject(Name=LevelData.CurrentLevelTag)] private LevelModel levelModel;
        public TileType TargetType { get; private set; }

        public bool Perform(Vector2Int coords)
        {
            levelModel.ReplaceTile(TargetType, coords, true);
            
            return true;

        }

        public void Dispose()
        {
            levelModel.Data.UnmarkAsInjected(this);
            levelModel = null;
            ObjectPool<PlaceTileAction>.Put(this);
        }

        public class Factory: IFactory<PlaceTileAction, TileType>
        {
            [Inject] private InjectorContext context;
            [Inject] private IFactory<PlaceTileAction> baseObjectFactory;

            public bool ManuallyInjected => false;

            public PlaceTileAction GetInstance(TileType type)
            {
                var instance = baseObjectFactory.GetInstance();

                instance.TargetType = type;
                context.Inject(instance);

                return instance;
            }

            public void Dispose()
            {
                baseObjectFactory.Dispose();
            }
        }
        
        public class ConcreteFactory: ObjectPoolFactory<PlaceTileAction>
        {
            public TileType TargetType { get; }

            public ConcreteFactory(TileType targetType)
            {
                TargetType = targetType;
            } 
            
            public override PlaceTileAction GetInstance()
            {
                var instance = base.GetInstance();

                instance.TargetType = TargetType;

                return instance;
            }
        }
    }
}