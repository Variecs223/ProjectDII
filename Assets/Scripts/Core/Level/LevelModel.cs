using System;
using System.Linq;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    [Serializable]
    public class LevelModel: IModel, IInjectable
    {
        [Inject] [SerializeField] protected LevelData data;
        [Inject] private IFactory<BaseTileModel, TileType> tileFactory;
        [Inject] private IFactory<IObjectPackage, ObjectType> objectFactory;

        [SerializeField] protected BaseTileModel[] tiles;
        public LevelData.ActionCategory[] actions;
        public int selectedAction;
        
        public BaseTileModel[] Tiles => tiles;

        public LevelData Data => data;
        InjectorContext IModel.ModelType => Data;

        public event Action<IObjectPackage> OnObjectAdded;

        public void OnInjected()
        {
            
        }

        public void Load()
        {
            tiles = new BaseTileModel[Data.tiles.Length];

            for (var i = 0; i < Tiles.Length; i++)
            {
                Tiles[i] = tileFactory.GetInstance(Data.tiles[i]);
            }

            foreach (var objectLocation in Data.objects)
            {
                AddObject(objectLocation.Type, objectLocation.Coords, objectLocation.Direction);
            }

            actions = Data.actions.ToArray();
        }

        public void AddObject(ObjectType type, Vector2Int coords, Direction dir = Direction.Up)
        {
            using var package = objectFactory.GetInstance(type);
            
            AddObject(package, coords, dir);
        }

        public void AddObject(IObjectPackage package, Vector2Int coords, Direction dir = Direction.Up)
        {
            package.GetModels(model =>
            {
                model.coords = coords + Vector2.one * 0.5f;
                model.direction = dir;
                Tiles[coords.y * Data.fieldSize.x + coords.x].objects.Add(
                    new BaseTileModel.ObjectTransitionState
                    {
                        Object = model,
                        State = TransitionState.Stationary
                    });
            });
                
            OnObjectAdded?.Invoke(package);
        }
        
        public void Dispose()
        {
            if (Data != null)
            {
                Data.UnmarkAsInjected(this);
            }

            foreach (var tile in tiles)
            {
                tile.Dispose();
            }

            tiles = null;
            actions = null;
        }
    }
}