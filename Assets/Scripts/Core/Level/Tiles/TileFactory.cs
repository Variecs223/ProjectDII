using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class TileFactory: IFactory<BaseTileModel, TileType>
    {
        [Inject] private InjectorContext context;
        
        public bool ManuallyInjected => true;

        private readonly Dictionary<TileType, IFactory<BaseTileModel>> concreteFactories = new Dictionary<TileType, IFactory<BaseTileModel>>();
        public IReadOnlyDictionary<TileType, IFactory<BaseTileModel>> ConcreteFactories => concreteFactories;

        public TileFactory()
        {
            concreteFactories.Add(TileType.EmptyTile, new ObjectPoolFactory<EmptyTileModel>());
            concreteFactories.Add(TileType.Wall, new ObjectPoolFactory<WallModel>());
        }
        
        public BaseTileModel GetInstance(TileType type)
        {
            if (!concreteFactories.ContainsKey(type))
            {
                Debug.LogError($"No factory found for tile type {type}");
                return null;
            }

            var tile = concreteFactories[type].GetInstance();
            
            if (!concreteFactories[type].ManuallyInjected)
            {
                context.Inject(tile);
            }

            return tile;
        }
        
        public void Dispose()
        {
            foreach (var concreteFactory in concreteFactories.Values)
            {
                concreteFactory.Dispose();
            }
            
            concreteFactories.Clear();
        }
    }
}