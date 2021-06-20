using System.Reflection;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection.Bindables;

namespace Variecs.ProjectDII.DependencyInjection.Conditions
{
    public class GameObjectCondition: ICondition
    {
        public GameObject TargetGameObject { get; protected set; }
        
        private IBindable<object> binding;
        private InjectorContext context;
        public GameObjectCondition Update<TBase>(BaseBinding<TBase> newBinding, GameObject targetGameObject) where TBase: class
        {
            TargetGameObject = targetGameObject;
            binding = newBinding;
            context = newBinding.Context;
            context.RegisterGameObjectBinding(TargetGameObject, binding);
            return this;
        }
        
        public bool IsFulfilled(object target, FieldInfo fieldInfo)
        {
            return target is Component component && IsGameObject(component.gameObject, TargetGameObject);
        }

        private static bool IsGameObject(GameObject source, GameObject target)
        {
            var parent = source.transform.parent;
            
            return source == target || parent != null && IsGameObject(parent.gameObject, target);
        }

        public void Dispose()
        {
            context.UnregisterGameObjectBinding(TargetGameObject, binding);
            context = null;
            binding = null;
            TargetGameObject = null;
            ObjectPool<GameObjectCondition>.Put(this);
        }
    }
}