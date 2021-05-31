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
    }
}