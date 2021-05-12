using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ProxyGameObjectBinding<TBase>: ProxyBinding<TBase> where TBase: Object
    {
        public PrefabInstanceBinding<TBase> ToPrefabInstance(TBase prefab)
        {
            Context.Unbind(this);
            var binding = ObjectPool<PrefabInstanceBinding<TBase>>.Get().Update(this, prefab);
            Context.Bind(binding);
            
            return binding;
        }
    }
}