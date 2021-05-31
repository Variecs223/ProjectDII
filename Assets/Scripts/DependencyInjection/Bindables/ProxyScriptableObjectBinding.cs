using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class ProxyScriptableObjectBinding<TBase>: ProxyBinding<TBase> where TBase: ScriptableObject
    {
        public ScriptableObjectInstanceBinding<TBase, TSpecific> ToScriptableObjectInstance<TSpecific>() where TSpecific: TBase
        {
            var context = Context;
            
            context.Unbind(this);
            var binding = ObjectPool<ScriptableObjectInstanceBinding<TBase, TSpecific>>.Get().Update(context, Conditions);
            context.Bind(binding);
            
            return binding;
        }
        
        public override IBindable<TBase> Clone()
        {
            var clone = ObjectPool<ProxyScriptableObjectBinding<TBase>>.Get();
            clone.Update(Context);
            clone.Conditions = Conditions;
            return clone;
        }
    }
}