using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Variecs.ProjectDII.DependencyInjection.Conditions;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public abstract class BaseBinding<TBase>: IBindable<TBase> where TBase : class
    {
        public IList<ICondition> Conditions { get; protected set; }

        public abstract TBase Inject();
        public abstract IBindable<TBase> Clone();

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

        public bool Temporary { get; protected set;  }

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
        
        public BaseBinding<TBase> SetTemporary()
        {
            Temporary = true;
            return this;
        }        
    }
}