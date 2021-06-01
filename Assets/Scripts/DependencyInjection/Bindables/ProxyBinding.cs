namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ProxyBinding<TBase>: BaseBinding<TBase> where TBase: class
    {
        public ProxyBinding<TBase> Update(InjectorContext newContext)
        {
            Context = newContext;
            
            return this;
        }

        public override TBase Inject()
        {
            return default;
        }

        public override void Dispose()
        {
            base.Dispose();

            Context = null;
            ObjectPool<ProxyBinding<TBase>>.Put(this);
        }
        
        public override IBindable<TBase> Clone()
        {
            var clone = ObjectPool<ProxyBinding<TBase>>.Get().Update(Context);
            clone.Conditions = Conditions;
            return clone;
        }

        public ValueBinding<TBase> ToValue(TBase value)
        {
            var context = Context;
            
            context.Unbind(this);
            var binding = ObjectPool<ValueBinding<TBase>>.Get().Update(context, Conditions, value);
            context.Bind(binding);
            
            return binding;
        }

        public SingletonBinding<TBase, TSpecific> ToSingleton<TSpecific>() where TSpecific : TBase, new()
        {
            var context = Context;
            
            context.Unbind(this);
            var binding = ObjectPool<SingletonBinding<TBase, TSpecific>>.Get().Update(context, Conditions);
            context.Bind(binding);
            
            return binding;
        }

        public ObjectInstanceBinding<TBase, TSpecific> ToObjectInstance<TSpecific>() where TSpecific : TBase, new()
        {
            var context = Context;
            
            context.Unbind(this);
            var binding = ObjectPool<ObjectInstanceBinding<TBase, TSpecific>>.Get().Update(context, Conditions);
            context.Bind(binding);
            
            return binding;
        }

        public FactoryBinding<TBase> ToFactory(IFactory<TBase> factory)
        {
            var context = Context;
            
            context.Unbind(this);
            var binding = ObjectPool<FactoryBinding<TBase>>.Get().Update(context, Conditions, factory);
            context.Bind(binding);
            
            return binding;
        }

        public NameBinding<TBase> ToName(string name)
        {
            var context = Context;
            
            context.Unbind(this);
            var binding = ObjectPool<NameBinding<TBase>>.Get().Update(context, Conditions, name);
            context.Bind(binding);
            
            return binding;
        }
    }
}