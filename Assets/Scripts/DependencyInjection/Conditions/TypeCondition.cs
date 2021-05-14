namespace Variecs.ProjectDII.DependencyInjection.Conditions
{
    public class TypeCondition<T>: ICondition
    {
        public bool IsFulfilled(object target)
        {
            return target.GetType() == typeof(T);
        }

        public void Dispose()
        {
            ObjectPool<TypeCondition<T>>.Put(this);
        }
    }
}