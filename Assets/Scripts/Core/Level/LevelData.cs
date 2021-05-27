using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "DII/Core/Level Data", order = 1)]
    public class LevelData : InjectorContext
    {
        [SerializeField] private GameObject viewPrefab;
        public Vector2Int fieldSize;
        public TileType[] tiles;
        
        [Inject] private IFactory<LevelModel> levelModelFactory;
        [Inject] private IFactory<LevelController, LevelModel> levelControllerFactory;
        [Inject] private IFactory<GameObject, LevelViewFactoryArgs> levelViewFactory;

        public void Init()
        {
            Inject(this);
        }
        
        protected override void PreInject()
        {
            base.PreInject();
            
            Bind<InjectorContext>().ToValue(this);
            Bind<LevelData>().ToValue(this);
            Bind<IFactory<LevelModel>>().ToSingleton<LevelModelFactory>();
            Bind<IFactory<LevelController, LevelModel>>().ToSingleton<LevelControllerFactory>();
            Bind<IFactory<GameObject, LevelViewFactoryArgs>>().ToSingleton<LevelViewFactory>();
            Bind<IFactory<BaseTileModel, TileType>>().ToSingleton<TileFactory>();
            Bind<GameObject>().ToValue(viewPrefab).ForType<LevelViewFactory>();
            Bind<Transform>().ToName("LevelContainer").ForType<LevelViewFactory>();
            MarkAsInjected(viewPrefab);
        }

        public LevelModel GetLevelModel()
        {
            return levelModelFactory.GetInstance();
        }

        public LevelController GetLevelController(LevelModel model)
        {
            return levelControllerFactory.GetInstance(model);
        }

        public GameObject GetLevelView(LevelModel model, LevelController controller)
        {
            return levelViewFactory.GetInstance(new LevelViewFactoryArgs { Model = model, Controller = controller });
        }

        public class LevelModelFactory: IFactory<LevelModel>
        {
            public bool ManuallyInjected => true;
            [field: Inject] public InjectorContext Context { get; }
            
            public LevelModel GetInstance()
            {
                var model = new LevelModel();
                Context.Inject(model);
                return model;
            }

            public void Dispose()
            {
                
            }
        }
        
        public class LevelControllerFactory : IFactory<LevelController, LevelModel> 
        {
            public bool ManuallyInjected => true;
            [field: Inject] public InjectorContext Context { get; }

            public LevelController GetInstance(LevelModel model)
            {
                var controller = new LevelController();
                Context.Bind<LevelModel>().ToValue(model).SetTemporary();
                Context.Inject(controller);
                Context.RemoveTemporaryBindings<LevelModel>();
                return controller;
            }

            public void Dispose()
            {
                
            }
        }
        
        public class LevelViewFactory : IFactory<GameObject, LevelViewFactoryArgs> 
        {
            public bool ManuallyInjected => true;
            [field: Inject] public InjectorContext Context { get; }
            [field: Inject] public GameObject ViewPrefab { get; }
            [field: Inject] public Transform ViewParent { get; }

            public GameObject GetInstance(LevelViewFactoryArgs args)
            {
                var view = Instantiate(ViewPrefab, ViewParent);
                Context.Bind<LevelModel>().ToValue(args.Model).SetTemporary();
                Context.Bind<LevelController>().ToValue(args.Controller).SetTemporary();

                foreach (var injectable in view.GetComponentsInChildren<IInjectable>())
                {
                    Context.Inject(injectable);
                }
                
                Context.RemoveTemporaryBindings<LevelModel>();
                Context.RemoveTemporaryBindings<LevelController>();
                return view;
            }

            public void Dispose()
            {
                
            }
        }

        public struct LevelViewFactoryArgs
        {
            public LevelModel Model;
            public LevelController Controller;
        }
    }
}