using System.Collections.Generic;
using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class PrefabInstanceBinding<TBase>: BaseBinding<TBase> 
        where TBase: Object 
    {
        public TBase Prefab { get; private set; }
        public Transform Parent { get; private set; }

        public PrefabInstanceBinding<TBase> Update(InjectorContext context, IList<ICondition> conditions, TBase prefab, Transform parent = null)
        {
            Prefab = prefab;
            Parent = parent;
            Context = context;
            Conditions = conditions;
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
            base.Dispose();

            Context = null;
            Prefab = null;
            Parent = null;
            ObjectPool<PrefabInstanceBinding<TBase>>.Put(this);
        }
        
        public override IBindable<TBase> Clone()
        {
            return ObjectPool<PrefabInstanceBinding<TBase>>.Get().Update(Context, Conditions, Prefab, Parent);
        }
    }
}