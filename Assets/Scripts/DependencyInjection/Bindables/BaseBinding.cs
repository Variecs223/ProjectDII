using System.Collections.Generic;
using System.Linq;
using Variecs.ProjectDII.DependencyInjection.Conditions;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public abstract class BaseBinding<TBase>: IBindable<TBase> where TBase : class
    {
        public List<ICondition> Conditions { get; protected set; }

        public abstract TBase Inject();

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

        public bool CheckConditions(object target)
        {
            return Conditions == null || Conditions.All(condition => condition.IsFulfilled(target));
        }

        public bool Temporary { get; protected set;  }

        public BaseBinding<TBase> ForType<TTarget>()
        {
            Conditions ??= new List<ICondition>();
            Conditions.Add(ObjectPool<TypeCondition<TTarget>>.Get());
            return this;
        }
        
        public BaseBinding<TBase> SetTemporary()
        {
            Temporary = true;
            return this;
        }        
    }
}