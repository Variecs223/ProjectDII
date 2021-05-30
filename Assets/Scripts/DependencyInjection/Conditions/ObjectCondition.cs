using System.Reflection;
using Variecs.ProjectDII.DependencyInjection.Bindables;

namespace Variecs.ProjectDII.DependencyInjection.Conditions
{
    public class ObjectCondition: ICondition
    {
        public object SelectedTarget { get; protected set; }

        private IBindable<object> binding;
        private InjectorContext context;
        
        public ObjectCondition Update<TBase>(BaseBinding<TBase> newBinding, object selectedTarget) where TBase: class
        {
            SelectedTarget = selectedTarget;
            binding = newBinding;
            context = newBinding.Context;
            context.RegisterObjectBinding(selectedTarget, binding);
            return this;
        }
        
        public bool IsFulfilled(object target, FieldInfo fieldInfo)
        {
            return target == SelectedTarget;
        }

        public void Dispose()
        {
            context.UnregisterObjectBinding(SelectedTarget, binding);
            context = null;
            binding = null;
            SelectedTarget = null;
            ObjectPool<ObjectCondition>.Put(this);
        }
    }
}