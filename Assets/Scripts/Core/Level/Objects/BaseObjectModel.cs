using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class BaseObjectModel: ScriptableObject, IModel
    {
        [Inject] public InjectorContext Data;
        
        InjectorContext IModel.ModelType => Data;

        public virtual void Dispose()
        {
            
        }

        public class Factory : IFactory<BaseObjectModel>
        {
            public bool ManuallyInjected => true;

            [Inject] private InjectorContext injectorContext;
            
            public BaseObjectModel GetInstance()
            {
                var model = CreateInstance<BaseObjectModel>();
                injectorContext.Inject(model);
                return model;
            }
            
            public void Dispose()
            {
                
            }
        }
    }
}