using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class PrefabInstanceBinding<TBase>: BaseBinding<TBase> 
        where TBase: Object 
    {
        public TBase Prefab { get; private set; }
        public InjectorContext Context { get; private set; }
        public Transform Parent { get; private set; }

        public PrefabInstanceBinding<TBase> Update(ProxyBinding<TBase> proxy, TBase prefab, Transform parent = null)
        {
            Prefab = prefab;
            Parent = parent;
            Context = proxy.Context;
            Conditions = proxy.Conditions;
            return this;
        }
        
        public override TBase Inject()
        {
            var newInstance = Object.Instantiate(Prefab, Parent);
            
            Context.Inject(newInstance);
            
            return newInstance;
        }

        public override void Dispose()
        {
            ObjectPool<PrefabInstanceBinding<TBase>>.Put(this);
        }
    }
}