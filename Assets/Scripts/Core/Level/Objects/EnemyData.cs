using System;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "DII/Core/Objects/Enemy", order = 1)]
    public class EnemyData : BaseObjectData, IFactory<EnemyData.EnemyPackage>
    {
        [SerializeField] private GameObject enemyPrefab;
        
        public float baseSpeed;
        public Direction[] directionPreferences;
        
        protected override void PreInject()
        {
            base.PreInject();

            Bind<EnemyData>().ToValue(this);
            Bind<BaseObjectData>().ToValue(this);
            BaseContext.Bind<BaseObjectData>().ToValue(this).ForList();
            Bind<GameObject>().ToValue(enemyPrefab).ForType<EnemyPackage>();
            Bind<Transform>().ToName(LevelLayoutView.ObjectContainerName).ForType<EnemyPackage>();
            MarkAsInjected(enemyPrefab);
        }

        EnemyPackage IFactory<EnemyPackage>.GetInstance()
        {
            return GetConcreteInstance();
        }

        public override IObjectPackage GetInstance()
        {
            return GetConcreteInstance();
        }

        public EnemyPackage GetConcreteInstance()
        {
            var package = ObjectPool<EnemyPackage>.Get();
            Inject(package);
            return package;
        }
        
        public class EnemyPackage: IObjectPackage
        {
            [Inject] private EnemyData enemyData;
            [Inject] private GameObject viewPrefab;
            [Inject] private Transform viewContainer;
            
            private BaseObjectModel enemyModel;
            private MovableController movableController;
            private EnemyAIController enemyAIController;
            private GameObject view;
            
            public void GetModels(Action<BaseObjectModel> modelAddition = null)
            {
                if (enemyModel == null)
                {
                    enemyModel = CreateInstance<BaseObjectModel>();
                    enemyData.Inject(enemyModel);
                }
                
                modelAddition?.Invoke(enemyModel);
            }

            public void GetControllers(Action<IController> controllerAddition = null)
            {
                if (enemyModel == null)
                {
                    GetModels();
                }

                if (movableController == null)
                {
                    movableController = new MovableController();
                    enemyData.Bind<BaseObjectModel>().ToValue(enemyModel).ForObject(movableController);
                    enemyData.Inject(movableController);
                }
                
                if (enemyAIController == null)
                {
                    enemyAIController = new EnemyAIController();
                    enemyData.Bind<BaseObjectModel>().ToValue(enemyModel).ForObject(enemyAIController);
                    enemyData.Bind<IMovable>().ToValue(movableController).ForObject(enemyAIController);
                    enemyData.Inject(enemyAIController);
                }
                
                controllerAddition?.Invoke(movableController);
                controllerAddition?.Invoke(enemyAIController);
            }

            public void GetViews(Action<GameObject> viewAddition = null)
            {
                if (enemyModel == null)
                {
                    GetModels();
                }

                if (movableController == null || enemyAIController == null)
                {
                    GetControllers();
                }

                if (view == null)
                {
                    view = Instantiate(viewPrefab, viewContainer);
                    enemyData.Bind<BaseObjectModel>().ToValue(enemyModel).ForGameObject(view);
                    enemyData.Bind<IMovable>().ToValue(movableController).ForGameObject(view);
                    enemyData.Bind<EnemyAIController>().ToValue(enemyAIController).ForGameObject(view);

                    foreach (var injectable in view.GetComponentsInChildren<IInjectable>())
                    {
                        enemyData.Inject(injectable);
                    }
                }
                
                viewAddition?.Invoke(view);
            }

            public void Dispose()
            {
                enemyData.UnmarkAsInjected(this);
                
                enemyData = null;
                viewPrefab = null;
                viewContainer = null;
                enemyModel = null;
                movableController = null;
                enemyAIController = null;
                view = null;
                ObjectPool<EnemyPackage>.Put(this);
            }
        }
    }
}