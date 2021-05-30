using System.Collections.Generic;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ValueBinding<TBase>: BaseBinding<TBase> where TBase: class
    {
        public TBase Value { get; private set; }

        public ValueBinding<TBase> Update(InjectorContext context, IList<ICondition> conditions, TBase value)
        {
            Value = value;
            Context = context;
            Conditions = conditions;
            
            return this;
        }
        
        public override TBase Inject()
        {
            Context.Inject(Value);

            return Value;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            Value = null;
            Context = null;
            ObjectPool<ValueBinding<TBase>>.Put(this);
        }
        
        public override IBindable<TBase> Clone()
        {
            return ObjectPool<ValueBinding<TBase>>.Get().Update(Context, Conditions, Value);
        }
    }
}