using System;
using System.Linq;
using Variecs.ProjectDII.DependencyInjection.Conditions;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ProxyBinding<TBase>: BaseBinding<TBase> where TBase: class
    {
        public Type Target { get; private set; }
        public InjectorContext Context { get; private set; }

        public ProxyBinding<TBase> Update(Type newTarget, InjectorContext newContext)
        {
            Target = newTarget;
            Context = newContext;
            
            return this;
        }

        public override TBase Inject()
        {
            return default;
        }

        public override void Dispose()
        {
            ObjectPool<ProxyBinding<TBase>>.Put(this);
        }
        
        public override IBindable<TBase> Clone()
        {
            var clone = ObjectPool<ProxyBinding<TBase>>.Get().Update(Target, Context);
            clone.Conditions = Conditions;
            return clone;
        }

        public ValueBinding<TBase> ToValue(TBase value)
        {
            Context.Unbind(this);
            var binding = ObjectPool<ValueBinding<TBase>>.Get().Update(Context, Conditions, value);
            Context.Bind(binding);
            
            return binding;
        }

        public SingletonBinding<TBase, TSpecific> ToSingleton<TSpecific>() where TSpecific : TBase, new()
        {
            Context.Unbind(this);
            var binding = ObjectPool<SingletonBinding<TBase, TSpecific>>.Get().Update(Context, Conditions);
            Context.Bind(binding);
            
            return binding;
        }

        public ObjectInstanceBinding<TBase, TSpecific> ToObjectInstance<TSpecific>() where TSpecific : TBase, new()
        {
            Context.Unbind(this);
            var binding = ObjectPool<ObjectInstanceBinding<TBase, TSpecific>>.Get().Update(Context, Conditions);
            Context.Bind(binding);
            
            return binding;
        }

        public FactoryBinding<TBase> ToFactory(IFactory<TBase> factory)
        {
            Context.Unbind(this);
            var binding = ObjectPool<FactoryBinding<TBase>>.Get().Update(Context, Conditions, factory);
            Context.Bind(binding);
            
            return binding;
        }

        public BaseBinding<TBase> ToName(string name)
        {
            NameCondition targetCondition = null;
            
            var newBindable = Context.ParentContext.FindBinding<TBase>(bindable =>
            {
                if (bindable is BaseBinding<TBase> binding)
                {
                    targetCondition = binding.Conditions
                        .FirstOrDefault(condition =>
                            condition is NameCondition nameCondition
                            && nameCondition.Name.Equals(name)) as NameCondition;
                    return targetCondition != null;
                }

                return false;
            });

            if (!(newBindable?.Clone() is BaseBinding<TBase> newBinding))
            {
                return null;
            }

            newBinding.Conditions.Remove(targetCondition);
            targetCondition.Dispose();

            return newBinding;
        }
    }
}