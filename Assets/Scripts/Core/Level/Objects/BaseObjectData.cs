using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public abstract class BaseObjectData : InjectorContext, IFactory<IObjectPackage>
    {
        public bool ManuallyInjected => true;
        public ObjectType objectType;
        public bool canCharge;
        public abstract IObjectPackage GetInstance();

        public override void Dispose()
        {
            base.Dispose();
            
            UnmarkAsInjected(this);
        }
    }
}