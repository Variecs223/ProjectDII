using System.Diagnostics;
using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ProxyGameObjectBinding<TBase>: ProxyBinding<TBase> where TBase: Object
    {
        public PrefabInstanceBinding<TBase> ToPrefabInstance(TBase prefab)
        {
            var context = Context;
            
            context.Unbind(this);
            var binding = ObjectPool<PrefabInstanceBinding<TBase>>.Get().Update(context, Conditions, prefab);
            context.Bind(binding);
            
            return binding;
        }
        
        public override IBindable<TBase> Clone()
        {
            var clone = ObjectPool<ProxyGameObjectBinding<TBase>>.Get();
            clone.Update(Context);
            clone.Conditions = Conditions;
            return clone;
        }
    }
}