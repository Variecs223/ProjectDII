using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelController: IController, IInjectable
    {
        [field: Inject] private LevelModel model;
        [Inject] private IFactory<IPlayerAction, PlayerActionType> playerActionFactory;

        protected readonly List<IController> ObjectControllers = new List<IController>();
        private readonly List<IController> removalList = new List<IController>();

        public void OnInjected()
        {
            model.OnObjectAdded += AddController;
            model.OnRemoved += Dispose;
        }

        protected void AddController(IObjectPackage package)
        {
            package.GetControllers(controller => ObjectControllers.Add(controller));
        }

        public void RemoveController(IController controller)
        {
            if (ObjectControllers.Contains(controller))
            {
                removalList.Add(controller);
            }
        }

        public void OnTileClick(Vector2Int coords)
        {
            if (model.actions[model.selectedAction].Amount <= 0)
            {
                return;
            }
            
            using var action = playerActionFactory.GetInstance(model.actions[model.selectedAction].Type);

            if (action.Perform(coords))
            {
                model.actions[model.selectedAction].Amount--;
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var controller in ObjectControllers)
            {
                controller.Update(deltaTime);
            }

            foreach (var controller in removalList.Where(controller => ObjectControllers.Contains(controller)))
            {
                ObjectControllers.Remove(controller);
            }

            if (removalList.Any())
            {
                removalList.Clear();
            }
        }

        public void Dispose()
        {
            if (model != null)
            {
                model.OnRemoved -= Dispose;
                model.OnObjectAdded -= AddController;
                
                if (model.Data != null)
                {
                    model.Data.UnmarkAsInjected(this);
                    model.Data.UnbindObject(this);
                }
            }

            foreach (var controller in ObjectControllers)
            {
                controller.Dispose();
            }

            ObjectControllers.Clear();
        }
    }
}