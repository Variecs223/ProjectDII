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
            Load();
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
                var package = objectFactory.GetInstance(objectLocation.Type);

                package?.AddModels(model =>
                {
                    Tiles[objectLocation.Coords.y * Data.fieldSize.x + objectLocation.Coords.x].objects.Add(
                        new BaseTileModel.ObjectTransitionState
                        {
                            Object = model,
                            State = TransitionState.Stationary
                        });
                });
                
                OnObjectAdded?.Invoke(package);
            }
        }
        
        public void Dispose()
        {
            Data.UnmarkAsInjected(this);
        }
    }
}