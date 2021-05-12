using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ScriptableObjectInstanceBinding<TBase, TSpecific>: IBindable<TBase> 
        where TBase: ScriptableObject 
        where TSpecific: TBase
    {
        public InjectorContext Context { get; private set; }

        public ScriptableObjectInstanceBinding<TBase, TSpecific> Update(ProxyBinding<TBase> proxy)
        {
            Context = proxy.Context;
            return this;
        }
        
        public TBase Inject()
        {
            var newInstance = ScriptableObject.CreateInstance<TSpecific>();
            
            Context.Inject(newInstance);
            
            return newInstance;
        }

        public bool CheckConditions()
        {
            return true;
        }

        public void Dispose()
        {
            ObjectPool<ScriptableObjectInstanceBinding<TBase, TSpecific>>.Put(this);
        }
    }
}