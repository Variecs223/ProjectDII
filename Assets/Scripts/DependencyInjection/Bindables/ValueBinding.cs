namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ValueBinding<TBase>: BaseBinding<TBase> where TBase: class
    {
        public TBase Value { get; private set; }
        public InjectorContext Context { get; private set; }

        private bool valueInjected;

        public ValueBinding<TBase> Update(ProxyBinding<TBase> proxy, TBase value, bool injectManually = false)
        {
            Value = value;
            Context = proxy.Context;
            Conditions = proxy.Conditions;
            valueInjected = injectManually;
            
            return this;
        }
        
        public override TBase Inject()
        {
            if (valueInjected)
            {
                return Value;
            }
            
            Context.Inject(Value);
            valueInjected = true;

            return Value;
        }

        public override void Dispose()
        {
            ObjectPool<ValueBinding<TBase>>.Put(this);
            valueInjected = false;
        }
    }
}