using UnityEditor;
using UnityEngine;

namespace Variecs.ProjectDII.Core.Bindables
{
    public class PrefabInstanceBinding<TBase>: IBindable<TBase> 
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
            return this;
        }
        
        public TBase Inject()
        {
            var newInstance = Object.Instantiate(Prefab, Parent);
            
            Context.Inject(newInstance);
            
            return newInstance;
        }

        public bool CheckConditions()
        {
            return true;
        }

        public void Dispose()
        {
            ObjectPool<PrefabInstanceBinding<TBase>>.Put(this);
        }
    }
}