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
        }
        
        [SerializeField] private GameObject viewPrefab;
        [Inject(Name="LevelContainer")] [SerializeField] private Transform viewContainer;
        public Vector2Int fieldSize;
        public TileType[] tiles;
        public ObjectLocation[] objects;
        
        protected override void PreInject()
        {
            base.PreInject();
            
            Bind<InjectorContext>().ToValue(this);
            Bind<LevelData>().ToValue(this);
            Bind<IFactory<BaseTileModel, TileType>>().ToSingleton<TileFactory>();
            Bind<IFactory<IObjectPackage, ObjectType>>().ToSingleton<ObjectFactory>();
            Bind<ObjectFactory>().ToSingleton<ObjectFactory>();
        }

        public LevelModel GetLevelModel()
        {
            var model = new LevelModel();
            Inject(model);
            return model;
        }

        public LevelController GetLevelController(LevelModel model)
        {
            var controller = new LevelController();
            Bind<LevelModel>().ToValue(model).ForObject(controller);
            Inject(controller);
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
                
            return view;
        }
    }
}