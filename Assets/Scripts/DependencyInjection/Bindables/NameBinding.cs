using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Variecs.ProjectDII.DependencyInjection.Conditions;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class NameBinding<TBase>: BaseBinding<TBase> where TBase: class
    {
        public string Name { get; private set; }

        private BaseBinding<TBase> currentBinding;
        
        public NameBinding<TBase> Update(InjectorContext context, IList<ICondition> conditions, string name)
        {
            Name = name;
            Context = context;
            Conditions = conditions;
            
            return this;
        }
        
        public override TBase Inject()
        {
            if (currentBinding == null)
            {
                return null;
            }
            
            var injection = currentBinding.Inject();

            currentBinding = null;
            
            return injection;
        }

        public override bool CheckConditions(object target, FieldInfo field)
        {
            if (!base.CheckConditions(target, field))
            {
                return false;
            }
            
            NameCondition targetCondition = null;
            
            var newBindable = Context.ParentContext.FindBinding<TBase>(bindable =>
            {
                
                if (bindable is BaseBinding<TBase> binding)
                {
                    targetCondition = binding.Conditions
                        .FirstOrDefault(condition =>
                            condition is NameCondition nameCondition
                            && nameCondition.Name.Equals(Name)) as NameCondition;
                    return targetCondition != null;
                }

                return false;
            });

            if (!(newBindable is BaseBinding<TBase> newBinding))
            {
                return false;
            }

            newBinding.Conditions.Remove(targetCondition);
            var result = newBinding.CheckConditions(target, field);
            newBinding.Conditions.Add(targetCondition);

            if (result)
            {
                currentBinding = newBinding;
            }
            
            return result;
        }

        public override void Dispose()
        {
            base.Dispose();

            Context = null;
            Name = null;
            ObjectPool<NameBinding<TBase>>.Put(this);
        }
        
        public override IBindable<TBase> Clone()
        {
            return ObjectPool<NameBinding<TBase>>.Get().Update(Context, Conditions, Name);
        }
    }
}