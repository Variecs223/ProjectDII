using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public abstract class BaseObjectData : InjectorContext, IFactory<IObjectPackage>
    {
        protected override void PreInject()
        {
            base.PreInject();

            Bind<InjectorContext>().ToValue(this);
            Bind<BaseObjectData>().ToValue(this);
        }

        public bool ManuallyInjected => true;
        public abstract ObjectType ObjectType { get; }
        public abstract IObjectPackage GetInstance();

        public override void Dispose()
        {
            base.Dispose();
            
            UnmarkAsInjected(this);
        }
    }
}