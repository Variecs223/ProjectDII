using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class TileFactory: IFactory<ITileModel, TileType>
    {
        [Inject] private InjectorContext context;
        
        public bool ManuallyInjected => true;

        private Dictionary<TileType, IFactory<ITileModel>> concreteFactories = new Dictionary<TileType, IFactory<ITileModel>>();
        public IReadOnlyDictionary<TileType, IFactory<ITileModel>> ConcreteFactories => concreteFactories;

        public TileFactory()
        {
            concreteFactories.Add(TileType.EmptyTile, new ConcreteTileFactory<EmptyTileModel>());
            concreteFactories.Add(TileType.Wall, new ConcreteTileFactory<WallModel>());
        }
        
        public ITileModel GetInstance(TileType type)
        {
            if (!concreteFactories.ContainsKey(type))
            {
                Debug.LogError($"No factory found for tile type {type}");
                return null;
            }

            var tile = concreteFactories[type].GetInstance();
            
            if (!concreteFactories[type].ManuallyInjected)
            {
                context.Inject(type);
            }

            return tile;
        }
        
        public void Dispose()
        {
            
        }
    }
}