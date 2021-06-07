using System;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class BaseObjectModel: ScriptableObject, IModel
    {
        [Inject] public BaseObjectData Data;
        
        InjectorContext IModel.ModelType => Data;

        public Vector2 coords;
        public Direction direction;
        public float speed;

        public event Action OnRemoved;
        
        public virtual bool AllowObject(ObjectType other)
        {
            return other != ObjectType.Box;
        }
        
        public virtual void Dispose()
        {
            OnRemoved?.Invoke();
        }
    }
}