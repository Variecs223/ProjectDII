using System;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ProxyBinding<TBase>: IBindable<TBase> where TBase: class
    {
        public Type Target { get; private set; }
        public InjectorContext Context { get; private set; }

        public ProxyBinding<TBase> Update(Type newTarget, InjectorContext newContext)
        {
            Target = newTarget;
            Context = newContext;
            
            return this;
        }

        public TBase Inject()
        {
            return default;
        }

        public bool CheckConditions()
        {
            return true;
        }

        public void Dispose()
        {
            ObjectPool<ProxyBinding<TBase>>.Put(this);
        }

        public ValueBinding<TBase> ToValue(TBase value)
        {
            Context.Unbind(this);
            var binding = ObjectPool<ValueBinding<TBase>>.Get().Update(this, value);
            Context.Bind(binding);
            
            return binding;
        }

        public SingletonBinding<TBase, TSpecific> ToSingleton<TSpecific>() where TSpecific : TBase, new()
        {
            Context.Unbind(this);
            var binding = ObjectPool<SingletonBinding<TBase, TSpecific>>.Get();
            Context.Bind(binding);
            
            return binding;
        }

        public ObjectInstanceBinding<TBase, TSpecific> ToObjectInstance<TSpecific>() where TSpecific : TBase, new()
        {
            Context.Unbind(this);
            var binding = ObjectPool<ObjectInstanceBinding<TBase, TSpecific>>.Get().Update(this);
            Context.Bind(binding);
            
            return binding;
        }

        public FactoryBinding<TBase> ToFactory(IFactory<TBase> factory)
        {
            Context.Unbind(this);
            var binding = ObjectPool<FactoryBinding<TBase>>.Get().Update(this, factory);
            Context.Bind(binding);
            
            return binding;
        }
    }
}