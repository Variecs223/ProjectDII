using System;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Actions;
using Variecs.ProjectDII.Core.Level.LevelEditor;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "DII/Core/Level Data", order = 1)]
    public class LevelData : InjectorContext
    {
        [Serializable]
        public struct ObjectLocation
        {
            public ObjectType Type;
            public Vector2Int Coords;
            public Direction Direction;
        }

        [Serializable]
        public struct ActionCategory
        {
            public ActionType Type;
            public int Amount;
        }

        public const string CurrentLevelTag = "CurrentLevel";
        public const string LevelContainerTag = "LevelContainer";
        
        [Inject(Optional=true)] [SerializeField] private GameObject viewPrefab;
        [Inject(Name=LevelContainerTag)] [SerializeField] private Transform viewContainer;
        public Vector2Int fieldSize;
        public TileType[] tiles;
        public ObjectLocation[] objects;
        public ActionCategory[] actions;
        public EndConditionType[] winConditions = { EndConditionType.Accumulator };
        public EndConditionType[] loseConditions = { EndConditionType.Overcharge, EndConditionType.ChargeCollision };
        public float minChargedDistance = 0.9f;
        
        public override void OnPreInjected()
        {
            base.OnPreInjected();
            
            Bind<InjectorContext>().ToValue(this);
            Bind<LevelData>().ToValue(this);
            Bind<IFactory<BaseTileModel, TileType>>().ToSingleton<TileFactory>();
            Bind<IFactory<IObjectPackage, ObjectType>>().ToSingleton<ObjectFactory>();
            Bind<IFactory<IAction, ActionType>>().ToSingleton<ActionFactory>();
            Bind<IFactory<IAction, ObjectType>>().ToSingleton<PlaceObjectAction.Factory>();
            Bind<IFactory<IAction, TileType>>().ToSingleton<PlaceTileAction.Factory>();
            Bind<IFactory<IEndCondition, EndConditionType>>().ToSingleton<EndConditionFactory>();
            Bind<IFactory<ObjectSelectionItemView, ObjectSelectionItemView.Package>>().ToSingleton<ObjectSelectionItemView.Factory>();
            Bind<IFactory<TileSelectionItemView, TileSelectionItemView.Package>>().ToSingleton<TileSelectionItemView.Factory>();
            Bind<IFactory<PlaceObjectAction>>().ToSingleton<ObjectPoolFactory<PlaceObjectAction>>().ForType<PlaceObjectAction.Factory>();
            Bind<IFactory<PlaceTileAction>>().ToSingleton<ObjectPoolFactory<PlaceTileAction>>().ForType<PlaceTileAction.Factory>();
        }

        public LevelModel GetLevelModel()
        {
            var model = new LevelModel();
            Inject(model);
            Bind<LevelModel>().ToValue(model).ForName(CurrentLevelTag);
            return model;
        }
        
        public LevelEditorModel GetLevelEditorModel(LevelModel model)
        {
            var editorModel = new LevelEditorModel();
            Bind<LevelModel>().ToValue(model).ForObject(editorModel);
            Inject(model);
            Bind<LevelEditorModel>().ToValue(editorModel).ForName(CurrentLevelTag);
            return editorModel;
        }

        public LevelController GetLevelController(LevelModel model)
        {
            var controller = new LevelController();
            Bind<LevelModel>().ToValue(model).ForObject(controller);
            Inject(controller);
            Bind<IObjectControllerContainer>().ToValue(controller).ForName(CurrentLevelTag);
            Bind<IEndConditionChecker>().ToValue(controller).ForName(CurrentLevelTag);
            return controller;
        }

        public LevelEditorController GetLevelEditorController(LevelModel model, LevelEditorModel editorModel)
        {
            var controller = new LevelEditorController();
            Bind<LevelModel>().ToValue(model).ForObject(controller);
            Bind<LevelEditorModel>().ToValue(editorModel).ForObject(controller);
            Inject(controller);
            Bind<IObjectControllerContainer>().ToValue(controller).ForName(CurrentLevelTag);
            Bind<IEndConditionChecker>().ForName(CurrentLevelTag);
            return controller;
        }

        public GameObject GetLevelView(LevelModel model, ITileClick controller, LevelEditorModel editorModel = null)
        {
            var view = Instantiate(viewPrefab, viewContainer);
            
            Bind<LevelModel>().ToValue(model).ForGameObject(view);
            Bind<LevelEditorModel>().ToValue(editorModel).ForGameObject(view);
            Bind<ITileClick>().ToValue(controller).ForGameObject(view);

            foreach (var injectable in view.GetComponentsInChildren<IInjectable>())
            {
                Inject(injectable);
            }
            
            Bind<GameObject>().ToValue(view).ForName(CurrentLevelTag);
                
            return view;
        }
    }
}