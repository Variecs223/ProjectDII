using System;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    [CreateAssetMenu(fileName = "BoxData", menuName = "DII/Core/Objects/Box", order = 0)]
    public class BoxData : InjectorContext, IFactory<BoxData.BoxPackage>
    {
        [SerializeField] private GameObject boxPrefab;

        protected override void PreInject()
        {
            base.PreInject();

            Bind<InjectorContext>().ToValue(this);
            BaseContext.Bind<BoxData>().ToValue(this);
            Bind<GameObject>().ToValue(boxPrefab).ForType<BoxPackage>();
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
            
            private BaseObjectModel baseObjectModel;
            private PushableController pushableController;
            private GameObject view;
            
            public void GetModels(Action<BaseObjectModel> modelAddition = null)
            {
                if (baseObjectModel == null)
                {
                    baseObjectModel = CreateInstance<BaseObjectModel>();
                    boxData.Inject(baseObjectModel);
                }
                
                modelAddition?.Invoke(baseObjectModel);
            }

            public void GetControllers(Action<IController> controllerAddition = null)
            {
                if (baseObjectModel == null)
                {
                    GetModels();
                }

                if (pushableController == null)
                {
                    pushableController = new PushableController();
                    boxData.Bind<BaseObjectModel>().ToValue(baseObjectModel).ForObject(pushableController);
                    boxData.Inject(pushableController);
                }
                
                controllerAddition?.Invoke(pushableController);
            }

            public void GetViews(Action<GameObject> viewAddition = null, Transform viewContainer = null)
            {
                if (baseObjectModel == null)
                {
                    GetModels();
                }

                if (pushableController == null)
                {
                    GetControllers();
                }

                if (view == null)
                {
                    view = Instantiate(viewPrefab, viewContainer);
                    boxData.Bind<BaseObjectModel>().ToValue(baseObjectModel).ForGameObject(view);
                    boxData.Bind<PushableController>().ToValue(pushableController).ForGameObject(view);

                    foreach (var injectable in view.GetComponentsInChildren<IInjectable>())
                    {
                        boxData.Inject(injectable);
                    }
                }
                
                viewAddition?.Invoke(view);
            }

            public void Dispose()
            {
                boxData.UnmarkAsInjected(this);
                
                boxData = null;
                viewPrefab = null;
                baseObjectModel = null;
                pushableController = null;
                view = null;
                ObjectPool<BoxPackage>.Put(this);
            }
        }
    }
}