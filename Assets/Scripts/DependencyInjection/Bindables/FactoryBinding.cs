using System.Collections.Generic;
using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection.Bindables
{
    public class FactoryBinding<TBase>: BaseBinding<TBase>
        where TBase: class
    {
        
        public InjectorContext Context { get; private set; }
        public IFactory<TBase> Factory { get; private set;  }
        
        public FactoryBinding<TBase> Update(InjectorContext context, IList<ICondition> conditions, IFactory<TBase> factory)
        {
            Context = context;
            Conditions = conditions;
            Factory = factory;
            return this;
        }
        
        public override TBase Inject()
        {
            var instance = Factory.GetInstance();

            if (!Factory.ManuallyInjected)
            {
                Context.Inject(instance);
            }
            
            return instance;
        }

        public override void Dispose()
        {
            ObjectPool<FactoryBinding<TBase>>.Put(this);
            Factory.Dispose();
        }
        
        public override IBindable<TBase> Clone()
        {
            return ObjectPool<FactoryBinding<TBase>>.Get().Update(Context, Conditions, Factory);
        }
    }
}