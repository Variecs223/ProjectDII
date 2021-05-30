using System;
using System.Reflection;

namespace Variecs.ProjectDII.DependencyInjection.Conditions
{
    public class PredicateCondition: ICondition
    {
        public Func<object, FieldInfo, bool> Predicate { get; private set; }

        public PredicateCondition Update(Func<object, FieldInfo, bool> predicate)
        {
            Predicate = predicate;
            return this;
        }
        
        public bool IsFulfilled(object target, FieldInfo fieldInfo)
        {
            return Predicate != null && Predicate.Invoke(target, fieldInfo);
        }

        public void Dispose()
        {
            Predicate = null;
            ObjectPool<PredicateCondition>.Put(this);
        }
    }
}