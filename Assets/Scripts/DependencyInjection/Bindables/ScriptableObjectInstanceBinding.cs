using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ScriptableObjectInstanceBinding<TBase, TSpecific>: BaseBinding<TBase>
        where TBase: ScriptableObject 
        where TSpecific: TBase
    {
        public InjectorContext Context { get; private set; }

        public ScriptableObjectInstanceBinding<TBase, TSpecific> Update(ProxyBinding<TBase> proxy)
        {
            Context = proxy.Context;
            Conditions = proxy.Conditions;
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
            ObjectPool<ScriptableObjectInstanceBinding<TBase, TSpecific>>.Put(this);
        }
    }
}