using System;

namespace Variecs.ProjectDII.Core.Bindables
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
            var binding = ObjectPool<ValueBinding<TBase>>.Get().Update(value);
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

        public MultitonBinding<TBase, TSpecific> ToMultiton<TSpecific>() where TSpecific : TBase, new()
        {
            Context.Unbind(this);
            var binding = ObjectPool<MultitonBinding<TBase, TSpecific>>.Get();
            Context.Bind(binding);
            
            return binding;
        }
    }
}