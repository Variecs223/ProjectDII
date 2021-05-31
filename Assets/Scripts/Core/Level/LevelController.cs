using System.Collections.Generic;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelController: IController, IInjectable
    {
        [field: Inject] private LevelModel model;

        protected readonly List<IController> ObjectControllers = new List<IController>();

        public void OnInjected()
        {
            model.OnObjectAdded += AddController;
        }

        protected void AddController(IObjectPackage package)
        {
            package.AddControllers(controller => ObjectControllers.Add(controller));
        }

        public void Update()
        {
            foreach (var controller in ObjectControllers)
            {
                controller.Update();
            }
        }

        public void Dispose()
        {
            model.Data.UnmarkAsInjected(this);
            model.Data.UnbindObject(this);

            foreach (var controller in ObjectControllers)
            {
                controller.Dispose();
            }

            ObjectControllers.Clear();
            
            model.OnObjectAdded -= AddController;
        }
    }
}