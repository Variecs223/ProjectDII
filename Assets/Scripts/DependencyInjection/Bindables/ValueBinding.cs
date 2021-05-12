namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ValueBinding<T>: IBindable<T> where T: class
    {
        public T Value { get; private set; }
        public InjectorContext Context { get; private set; }

        private bool valueInjected;

        public ValueBinding<T> Update(ProxyBinding<T> proxy, T value)
        {
            Value = value;
            Context = proxy.Context;
            valueInjected = false;
            
            return this;
        }
        
        public T Inject()
        {
            if (valueInjected)
            {
                return Value;
            }
            
            Context.Inject(Value);
            valueInjected = true;

            return Value;
        }

        public bool CheckConditions()
        {
            return true;
        }

        public void Dispose()
        {
            ObjectPool<ValueBinding<T>>.Put(this);
            valueInjected = false;
        }
    }
}