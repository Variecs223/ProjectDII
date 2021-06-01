﻿using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class ObjectFactory: IFactory<IObjectPackage, ObjectType>, IInjectable
    {
        [Inject] private InjectorContext context;
        [Inject] private BoxData boxData;
        
        private readonly Dictionary<ObjectType, IFactory<IObjectPackage>> concreteFactories = new Dictionary<ObjectType, IFactory<IObjectPackage>>();
        public IReadOnlyDictionary<ObjectType, IFactory<IObjectPackage>> ConcreteFactories => concreteFactories;
        
        public void OnInjected()
        {
            concreteFactories.Add(ObjectType.Box, boxData);
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