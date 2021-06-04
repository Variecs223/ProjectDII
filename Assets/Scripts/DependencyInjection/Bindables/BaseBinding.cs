using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection.Conditions;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public abstract class BaseBinding<TBase>: IBindable<TBase> where TBase : class
    {
        public InjectorContext Context { get; protected set; }
        public IList<ICondition> Conditions { get; protected set; }

        public abstract TBase Inject();
        public abstract IBindable<TBase> Clone();
        
        public Type GetBindedType()
        {
            return typeof(TBase);
        }
        
        public virtual void Dispose()
        {
            if (Conditions == null)
            {
                return;
            }
            
            foreach (var condition in Conditions)
            {
                condition.Dispose();
            }
            
            Conditions.Clear();
        }
        
        public virtual bool CheckConditions(object target, FieldInfo field)
        {
            return Conditions == null || Conditions.All(condition => condition.IsFulfilled(target, field));
        }

        public BaseBinding<TBase> ForObject(object target)
        {
            Conditions ??= new List<ICondition>();
            Conditions.Add(ObjectPool<ObjectCondition>.Get().Update(this, target));
            return this;
        }

        public BaseBinding<TBase> ForGameObject(GameObject gameObject)
        {
            Conditions ??= new List<ICondition>();
            Conditions.Add(ObjectPool<GameObjectCondition>.Get().Update(this, gameObject));
            return this;
        }
        
        public BaseBinding<TBase> ForType<TTarget>()
        {
            Conditions ??= new List<ICondition>();
            Conditions.Add(ObjectPool<TypeCondition<TTarget>>.Get());
            return this;
        }

        public BaseBinding<TBase> ForName(string name)
        {
            Conditions ??= new List<ICondition>();
            Conditions.Add(ObjectPool<NameCondition>.Get().Update(name));
            return this;
        }

        public BaseBinding<TBase> ForPredicate(Func<object, FieldInfo, bool> predicate)
        {
            Conditions ??= new List<ICondition>();
            Conditions.Add(ObjectPool<PredicateCondition>.Get().Update(predicate));
            return this;
        }

        public BaseBinding<TBase> ForList()
        {
            Conditions ??= new List<ICondition>();
            Conditions.Add(ObjectPool<ListCondition>.Get());
            return this;
        }
    }
}