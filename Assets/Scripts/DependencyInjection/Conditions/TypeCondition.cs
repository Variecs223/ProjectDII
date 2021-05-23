using System.Reflection;

namespace Variecs.ProjectDII.DependencyInjection.Conditions
{
    public class TypeCondition<T>: ICondition
    {
        public bool IsFulfilled(object target, FieldInfo fieldInfo)
        {
            return target.GetType() == typeof(T);
        }

        public void Dispose()
        {
            ObjectPool<TypeCondition<T>>.Put(this);
        }
    }
}