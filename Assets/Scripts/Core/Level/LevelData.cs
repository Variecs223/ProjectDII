using System;
using UnityEngine;
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
            public PlayerActionType Type;
            public int Amount;
        }

        public const string CurrentLevelTag = "CurrentLevel";
        
        [SerializeField] private GameObject viewPrefab;
        [Inject(Name="LevelContainer")] [SerializeField] private Transform viewContainer;
        public Vector2Int fieldSize;
        public TileType[] tiles;
        public ObjectLocation[] objects;
        public ActionCategory[] actions;
        
        protected override void PreInject()
        {
            base.PreInject();
            
            Bind<InjectorContext>().ToValue(this);
            Bind<LevelData>().ToValue(this);
            Bind<IFactory<BaseTileModel, TileType>>().ToSingleton<TileFactory>();
            Bind<IFactory<IObjectPackage, ObjectType>>().ToSingleton<ObjectFactory>();
            Bind<IFactory<IPlayerAction, PlayerActionType>>().ToSingleton<PlayerActionFactory>();
        }

        public LevelModel GetLevelModel()
        {
            var model = new LevelModel();
            Inject(model);
            Bind<LevelModel>().ToValue(model).ForName(CurrentLevelTag);
            return model;
        }

        public LevelController GetLevelController(LevelModel model)
        {
            var controller = new LevelController();
            Bind<LevelModel>().ToValue(model).ForObject(controller);
            Inject(controller);
            Bind<LevelController>().ToValue(controller).ForName(CurrentLevelTag);
            return controller;
        }

        public GameObject GetLevelView(LevelModel model, LevelController controller)
        {
            var view = Instantiate(viewPrefab, viewContainer);
            
            Bind<LevelModel>().ToValue(model).ForGameObject(view);
            Bind<LevelController>().ToValue(controller).ForGameObject(view);

            foreach (var injectable in view.GetComponentsInChildren<IInjectable>())
            {
                Inject(injectable);
            }
            
            Bind<GameObject>().ToValue(view).ForName(CurrentLevelTag);
                
            return view;
        }
    }
}