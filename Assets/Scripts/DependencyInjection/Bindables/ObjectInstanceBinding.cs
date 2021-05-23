using System.Collections.Generic;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ObjectInstanceBinding<TBase, TSpecific>: BaseBinding<TBase> 
        where TBase: class 
        where TSpecific: TBase, new()
    {
        public InjectorContext Context { get; private set; }

        public ObjectInstanceBinding<TBase, TSpecific> Update(InjectorContext context, IList<ICondition> conditions)
        {
            Context = context;
            Conditions = conditions;
            return this;
        }
        
        public override TBase Inject()
        {
            var newInstance = new TSpecific();
            
            Context.Inject(newInstance);
            
            return newInstance;
        }

        public override void Dispose()
        {
            ObjectPool<ObjectInstanceBinding<TBase, TSpecific>>.Put(this);
        }
        
        public override IBindable<TBase> Clone()
        {
            return ObjectPool<ObjectInstanceBinding<TBase, TSpecific>>.Get().Update(Context, Conditions);
        }
    }
}