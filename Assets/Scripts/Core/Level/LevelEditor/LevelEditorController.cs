using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Actions;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.LevelEditor
{
    public class LevelEditorController: IController, IInjectable, ITileClick, IObjectControllerContainer
    {
        [Inject] private LevelModel model;
        [Inject] private LevelEditorModel editorModel;
        [Inject] private IFactory<IAction, PlaceObjectAction.Package> placeObjectActionFactory;
        [Inject] private IFactory<IAction, TileType> placeTileActionFactory;

        public List<IController> ObjectControllers { get; } = new List<IController>();
        private readonly List<IController> removalList = new List<IController>();
        
        public void OnInjected()
        {
            model.OnRemoved += Dispose;
            model.OnObjectAdded += AddController;
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
            if (editorModel.SelectedObject != ObjectType.None)
            {
                using var action = placeObjectActionFactory.GetInstance(new PlaceObjectAction.Package
                {
                    type = editorModel.SelectedObject, 
                    direction = editorModel.SelectedDirection
                });

                action.Perform(coords);
            }
            
            if (editorModel.SelectedTile != TileType.None)
            {
                using var action = placeTileActionFactory.GetInstance(editorModel.SelectedTile);

                action.Perform(coords);
            }
        }

        public void Update(float deltaTime)
        {
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
                model.OnObjectAdded -= AddController;
                model.OnRemoved -= Dispose;
                
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