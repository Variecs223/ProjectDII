using System.Collections.Generic;

namespace Variecs.ProjectDII.Core.Level
{
    public interface IObjectControllerContainer
    {
        List<IController> ObjectControllers { get; }
        
        void AddController(IObjectPackage package);
        void RemoveController(IController controller);
    }
}