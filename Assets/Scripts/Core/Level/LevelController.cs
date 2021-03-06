using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelController: IController, IInjectable, ITileClick, IObjectControllerContainer, IEndConditionChecker
    {
        [Inject] private LevelModel model;
        [Inject] private IFactory<IAction, ActionType> playerActionFactory;

        public List<IController> ObjectControllers { get; } = new List<IController>();
        private readonly List<IController> removalList = new List<IController>();


        private bool tempState;
        
        public void OnInjected()
        {
            model.OnObjectAdded += AddController;
            model.OnRemoved += Dispose;
        }

        public void AddController(IObjectPackage package)
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

        public void TileClick(Vector2Int coords)
        {
            if (tempState || model.actions[model.selectedAction].Amount <= 0)
            {
                return;
            }
            
            using var action = playerActionFactory.GetInstance(model.actions[model.selectedAction].Type);

            if (action.Perform(coords))
            {
                model.actions[model.selectedAction].Amount--;
            }
        }

        public bool CheckVictory(EndConditionType type)
        {
            if (!model.WinConditions.ContainsKey(type) || !model.WinConditions[type].Check())
            {
                return false;
            }
            
            tempState = true;
            return true;

        }

        public bool CheckDefeat(EndConditionType type)
        {
            if (!model.LoseConditions.ContainsKey(type) || !model.LoseConditions[type].Check())
            {
                return false;
            }
            
            tempState = true;
            return true;
        }

        public void Update(float deltaTime)
        {
            if (!tempState)
            {
                foreach (var controller in ObjectControllers)
                {
                    controller.Update(deltaTime);
                }
                
                CheckDefeat(EndConditionType.ChargeCollision);
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