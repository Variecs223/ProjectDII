namespace Variecs.ProjectDII.Core.Bindables
{
    public class ValueBinding<T>: IBindable<T> where T: class
    {
        public T Value { get; private set; }

        public ValueBinding<T> Update(T value)
        {
            Value = value;
            return this;
        }
        
        public T Inject()
        {
            return Value;
        }

        public bool CheckConditions()
        {
            return true;
        }

        public void Dispose()
        {
            ObjectPool<ValueBinding<T>>.Put(this);
        }
    }
}