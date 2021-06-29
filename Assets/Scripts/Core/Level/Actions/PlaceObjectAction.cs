using System;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Actions
{
    public class PlaceObjectAction: IAction
    {
        [Inject(Name=LevelData.CurrentLevelTag)] private LevelModel levelModel;
        public ObjectType TargetType { get; private set; }
        public Direction TargetDirection { get; private set; } = Direction.Up;

        public bool Perform(Vector2Int coords)
        {
            if (!levelModel.tiles[coords.y * levelModel.Data.fieldSize.x + coords.x].AllowObject(TargetType))
            {
                return false;
            }
            
            levelModel.AddObject(TargetType, coords, TargetDirection);
            return true;

        }

        public void Dispose()
        {
            levelModel.Data.UnmarkAsInjected(this);
            levelModel = null;
            ObjectPool<PlaceObjectAction>.Put(this);
        }

        [Serializable]
        public struct Package
        {
            public ObjectType type;
            public Direction direction;
        }
        
        public class Factory: IFactory<PlaceObjectAction, Package>
        {
            [Inject] private InjectorContext context;
            [Inject] private IFactory<PlaceObjectAction> baseObjectFactory;

            public bool ManuallyInjected => false;

            public PlaceObjectAction GetInstance(Package package)
            {
                var instance = baseObjectFactory.GetInstance();

                instance.TargetType = package.type;
                instance.TargetDirection = package.direction;
                context.Inject(instance);

                return instance;
            }

            public void Dispose()
            {
                baseObjectFactory.Dispose();
            }
        }
        
        public class ConcreteFactory: ObjectPoolFactory<PlaceObjectAction>
        {
            public ObjectType TargetType { get; }

            public ConcreteFactory(ObjectType targetType)
            {
                TargetType = targetType;
            } 
            
            public override PlaceObjectAction GetInstance()
            {
                var instance = base.GetInstance();

                instance.TargetType = TargetType;

                return instance;
            }
        }
    }
}