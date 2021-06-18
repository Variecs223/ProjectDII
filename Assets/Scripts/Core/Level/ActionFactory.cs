using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Actions;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class ActionFactory: IFactory<IAction, ActionType>
    {
        [Inject] private InjectorContext context;
        
        public bool ManuallyInjected => true;

        private readonly Dictionary<ActionType, IFactory<IAction>> concreteFactories = new Dictionary<ActionType, IFactory<IAction>>();
        public IReadOnlyDictionary<ActionType, IFactory<IAction>> ConcreteFactories => concreteFactories;

        public ActionFactory()
        {
            concreteFactories.Add(ActionType.PlaceBox, new PlaceObjectAction.ConcreteFactory(ObjectType.Box));
        }
        
        public IAction GetInstance(ActionType type)
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