using System;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class BoxData : InjectorContext, IFactory<BoxData.BoxPackage>
    {
        [SerializeField] private GameObject boxPrefab;

        protected void Awake()
        {
            Init();
        }
        
        protected override void PreInject()
        {
            base.PreInject();

            Bind<InjectorContext>().ToValue(this);
            Bind<GameObject>().ToValue(boxPrefab).ForType<BoxPackage>();
            Bind<Transform>().ToName("ObjectContainer").ForType<BoxPackage>();
            MarkAsInjected(boxPrefab);
        }

        public bool ManuallyInjected => true;
        
        public BoxPackage GetInstance()
        {
            var package = ObjectPool<BoxPackage>.Get();
            Inject(package);
            return package;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            UnmarkAsInjected(this);
        }
        
        public class BoxPackage: IObjectPackage
        {
            [Inject] private BoxData boxData;
            [Inject] private GameObject viewPrefab;
            [Inject(Optional=true)] private Transform viewContainer;
            
            private BaseObjectModel baseObjectModel;
            private PushableController pushableController;
            private GameObject view;
            
            public void AddModels(Action<BaseObjectModel> modelAddition = null)
            {
                if (baseObjectModel == null)
                {
                    baseObjectModel = CreateInstance<BaseObjectModel>();
                    boxData.Inject(baseObjectModel);
                }
                
                modelAddition?.Invoke(baseObjectModel);
            }

            public void AddControllers(Action<IController> controllerAddition = null)
            {
                if (baseObjectModel == null)
                {
                    AddModels();
                }

                if (pushableController == null)
                {
                    pushableController = new PushableController();
                    boxData.Bind<BaseObjectModel>().ToValue(baseObjectModel).ForObject(pushableController);
                    boxData.Inject(pushableController);
                }
                
                controllerAddition?.Invoke(pushableController);
            }

            public void AddViews(Action<GameObject> viewAddition = null)
            {
                if (baseObjectModel == null)
                {
                    AddModels();
                }

                if (pushableController == null)
                {
                    AddControllers();
                }

                if (view == null)
                {
                    view = Instantiate(viewPrefab, viewContainer);
                    boxData.Bind<BaseObjectModel>().ToValue(baseObjectModel).ForGameObject(view);
                    boxData.Bind<PushableController>().ToValue(pushableController).ForGameObject(view);
                    boxData.Inject(view);
                }
                
                viewAddition?.Invoke(view);
            }

            public void Dispose()
            {
                boxData.UnmarkAsInjected(this);
                
                boxData = null;
                viewPrefab = null;
                viewContainer = null;
                baseObjectModel = null;
                pushableController = null;
                view = null;
                ObjectPool<BoxPackage>.Put(this);
            }
        }
    }
}