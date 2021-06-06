using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelController: IController, IInjectable
    {
        [field: Inject] private LevelModel model;
        [Inject] private IFactory<IPlayerAction, PlayerActionType> playerActionFactory;

        protected readonly List<IController> ObjectControllers = new List<IController>();

        public void OnInjected()
        {
            model.OnObjectAdded += AddController;
        }

        protected void AddController(IObjectPackage package)
        {
            package.GetControllers(controller => ObjectControllers.Add(controller));
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
        }

        public void Dispose()
        {
            if (model != null)
            {
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