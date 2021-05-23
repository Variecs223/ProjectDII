using System.Collections.Generic;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class SingletonBinding<TBase, TSpecific>: BaseBinding<TBase>
        where TBase: class 
        where TSpecific: TBase, new()
    {
        public static readonly TSpecific Instance = new TSpecific();

        public InjectorContext Context { get; private set; }

        public SingletonBinding<TBase, TSpecific> Update(InjectorContext context, IList<ICondition> conditions)
        {
            Context = context;
            Conditions = conditions;
            return this;
        }
        
        public override TBase Inject()
        {
            Context.Inject(Instance);
            
            return Instance;
        }

        public override void Dispose()
        {
            ObjectPool<SingletonBinding<TBase, TSpecific>>.Put(this);
        }
        
        public override IBindable<TBase> Clone()
        {
            return ObjectPool<SingletonBinding<TBase, TSpecific>>.Get().Update(Context, Conditions);
        }
    }
}