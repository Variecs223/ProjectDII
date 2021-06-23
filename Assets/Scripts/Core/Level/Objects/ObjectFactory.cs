using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class ObjectFactory: IFactory<IObjectPackage, ObjectType>, IInjectable
    {
        [Inject] private InjectorContext context;
        [InjectList] private List<BaseObjectData> objectDatas;
        
        private readonly Dictionary<ObjectType, IFactory<IObjectPackage>> concreteFactories = new Dictionary<ObjectType, IFactory<IObjectPackage>>();
        public IReadOnlyDictionary<ObjectType, IFactory<IObjectPackage>> ConcreteFactories => concreteFactories;
        
        public void OnInjected()
        {
            foreach (var objectData in objectDatas)
            {
                concreteFactories.Add(objectData.objectType, objectData);
            }
        }

        public bool ManuallyInjected => true;

        public IObjectPackage GetInstance(ObjectType type)
        {
            if (!concreteFactories.ContainsKey(type))
            {
                Debug.LogError($"No factory found for object type {type}");
                return null;
            }

            var package = concreteFactories[type].GetInstance();
            
            if (!concreteFactories[type].ManuallyInjected)
            {
                context.Inject(type);
            }

            return package;
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