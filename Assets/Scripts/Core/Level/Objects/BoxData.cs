using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class BoxData : InjectorContext
    {
        [Inject] private IFactory<BaseObjectModel> baseObjectModelFactory;
        
        public void Init()
        {
            Inject(this);
        }
        
        protected override void PreInject()
        {
            base.PreInject();

            Bind<InjectorContext>().ToValue(this);
            Bind<BoxData>().ToValue(this);
            Bind<IFactory<BaseObjectModel>>().ToSingleton<BaseObjectModel.Factory>();
        }
    }
}