using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.EndConditions;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class EndConditionFactory: IFactory<IEndCondition, EndConditionType>
    {
        [Inject] private InjectorContext context;
        
        public bool ManuallyInjected => true;

        private readonly Dictionary<EndConditionType, IFactory<IEndCondition>> concreteFactories = new Dictionary<EndConditionType, IFactory<IEndCondition>>();
        public IReadOnlyDictionary<EndConditionType, IFactory<IEndCondition>> ConcreteFactories => concreteFactories;

        public EndConditionFactory()
        {
            concreteFactories.Add(EndConditionType.Accumulator, new ObjectPoolFactory<AccumulatorWinCondition>());
            concreteFactories.Add(EndConditionType.Overcharge, new ObjectPoolFactory<OverchargeLoseCondition>());
            concreteFactories.Add(EndConditionType.ChargeCollision, new ObjectPoolFactory<ChargeCollisionLoseCondition>());
        }
        
        public IEndCondition GetInstance(EndConditionType type)
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