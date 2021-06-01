using System.Collections.Generic;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class SingletonBinding<TBase, TSpecific>: BaseBinding<TBase>
        where TBase: class 
        where TSpecific: TBase, new()
    {
        public SingletonBinding<TBase, TSpecific> Update(InjectorContext context, IList<ICondition> conditions)
        {
            Context = context;
            Conditions = conditions;
            return this;
        }
        
        public override TBase Inject()
        {
            var instance = SingletonHolder.Get<TSpecific>();
            
            Context.Inject(instance);
            
            return instance;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            Context = null;
            ObjectPool<SingletonBinding<TBase, TSpecific>>.Put(this);
        }
        
        public override IBindable<TBase> Clone()
        {
            return ObjectPool<SingletonBinding<TBase, TSpecific>>.Get().Update(Context, Conditions);
        }
    }
}