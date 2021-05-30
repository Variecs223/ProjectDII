using System.Collections.Generic;
using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ScriptableObjectInstanceBinding<TBase, TSpecific>: BaseBinding<TBase>
        where TBase: ScriptableObject 
        where TSpecific: TBase
    {
        public ScriptableObjectInstanceBinding<TBase, TSpecific> Update(InjectorContext context, IList<ICondition> conditions)
        {
            Context = context;
            Conditions = conditions;
            return this;
        }
        
        public override TBase Inject()
        {
            var newInstance = ScriptableObject.CreateInstance<TSpecific>();
            
            Context.Inject(newInstance);
            
            return newInstance;
        }

        public override void Dispose()
        {
            base.Dispose();

            Context = null;
            ObjectPool<ScriptableObjectInstanceBinding<TBase, TSpecific>>.Put(this);
        }
        
        public override IBindable<TBase> Clone()
        {
            return ObjectPool<ScriptableObjectInstanceBinding<TBase, TSpecific>>.Get().Update(Context, Conditions);
        }
    }
}