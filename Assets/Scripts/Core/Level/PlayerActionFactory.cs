using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Actions;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class PlayerActionFactory: IFactory<IPlayerAction, PlayerActionType>
    {
        [Inject] private InjectorContext context;
        
        public bool ManuallyInjected => true;

        private readonly Dictionary<PlayerActionType, IFactory<IPlayerAction>> concreteFactories = new Dictionary<PlayerActionType, IFactory<IPlayerAction>>();
        public IReadOnlyDictionary<PlayerActionType, IFactory<IPlayerAction>> ConcreteFactories => concreteFactories;

        public PlayerActionFactory()
        {
            concreteFactories.Add(PlayerActionType.PlaceBox, new ObjectPoolFactory<PlaceBoxPlayerAction>());
        }
        
        public IPlayerAction GetInstance(PlayerActionType type)
        {
            if (!concreteFactories.ContainsKey(type))
            {
                Debug.LogError($"No factory found for tile type {type}");
                return null;
            }

            var action = concreteFactories[type].GetInstance();
            
            if (!concreteFactories[type].ManuallyInjected)
            {
                context.Inject(action);
            }

            return action;
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