using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class PushableController: IController
    {
        [field: Inject] public BaseObjectModel Model { get; protected set; } 
        
        public void Update()
        {
            
        }
        
        public void Dispose()
        {
            Model.Data.UnmarkAsInjected(this);
            Model.Data.UnbindObject(this);
            Model = null;
        }
    }
}