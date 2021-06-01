using System;
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
                AddObject(objectLocation.Type, objectLocation.Coords);
            }
        }

        public void AddObject(ObjectType type, Vector2Int coords)
        {
            using var package = objectFactory.GetInstance(type);
                
            package.GetModels(model =>
            {
                model.coords = coords + Vector2.one * 0.5f;
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
        }
    }
}