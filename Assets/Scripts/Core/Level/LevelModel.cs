using System;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    [Serializable]
    public class LevelModel: IModel
    {
        [field: Inject] public LevelData Data { get; }
        [Inject] private TileFactory tileFactory;

        public ITileModel[] Tiles { get; protected set; }

        ScriptableObject IModel.ModelType => Data;

        public void Load()
        {
            Tiles = new ITileModel[Data.tiles.Length];

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