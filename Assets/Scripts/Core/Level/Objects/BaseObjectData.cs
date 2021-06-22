using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public abstract class BaseObjectData : InjectorContext, IFactory<IObjectPackage>
    {
        public bool ManuallyInjected => true;
        public ObjectType objectType;
        public bool canCharge;
        public Sprite icon;
        public abstract IObjectPackage GetInstance();

        public override void OnPreInjected()
        {
            base.OnPreInjected();
            
            Bind<BaseObjectData>().ToValue(this);
            BaseContext.Bind<BaseObjectData>().ToValue(this).ForList();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            UnmarkAsInjected(this);
        }
    }
}