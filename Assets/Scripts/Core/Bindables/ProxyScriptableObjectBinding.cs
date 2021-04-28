using UnityEngine;

namespace Variecs.ProjectDII.Core.Bindables
{
    public class ProxyScriptableObjectBinding<TBase>: ProxyBinding<TBase> where TBase: ScriptableObject
    {
        public ScriptableObjectInstanceBinding<TBase, TSpecific> ToScriptableObjectInstance<TSpecific>() where TSpecific: TBase
        {
            Context.Unbind(this);
            var binding = ObjectPool<ScriptableObjectInstanceBinding<TBase, TSpecific>>.Get().Update(this);
            Context.Bind(binding);
            
            return binding;
        }
    }
}