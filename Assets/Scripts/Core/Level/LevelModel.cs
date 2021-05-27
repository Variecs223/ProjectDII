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

        [SerializeField] protected BaseTileModel[] tiles;
        public BaseTileModel[] Tiles => tiles;

        public LevelData Data => data;
        ScriptableObject IModel.ModelType => Data;

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
        }
        
        public void Dispose()
        {
            Data.UnmarkAsInjected(this);
        }
    }
}