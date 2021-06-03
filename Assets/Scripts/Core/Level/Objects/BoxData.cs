using System;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    [CreateAssetMenu(fileName = "BoxData", menuName = "DII/Core/Objects/Box", order = 0)]
    public class BoxData : BaseObjectData, IFactory<BoxData.BoxPackage>
    {
        public override ObjectType ObjectType => ObjectType.Box;

        [SerializeField] private GameObject boxPrefab;

        protected override void PreInject()
        {
            base.PreInject();

            BaseContext.Bind<BoxData>().ToValue(this);
            Bind<GameObject>().ToValue(boxPrefab).ForType<BoxPackage>();
            MarkAsInjected(boxPrefab);
        }

        BoxPackage IFactory<BoxPackage>.GetInstance()
        {
            return GetConcreteInstance();
        }

        public override IObjectPackage GetInstance()
        {
            return GetConcreteInstance();
        }

        public BoxPackage GetConcreteInstance()
        {
            var package = ObjectPool<BoxPackage>.Get();
            Inject(package);
            return package;
        }
        
        public class BoxPackage: IObjectPackage
        {
            [Inject] private BoxData boxData;
            [Inject] private GameObject viewPrefab;
            
            private BoxModel boxModel;
            private PushableController pushableController;
            private GameObject view;
            
            public void GetModels(Action<BaseObjectModel> modelAddition = null)
            {
                if (boxModel == null)
                {
                    boxModel = CreateInstance<BoxModel>();
                    boxData.Inject(boxModel);
                }
                
                modelAddition?.Invoke(boxModel);
            }

            public void GetControllers(Action<IController> controllerAddition = null)
            {
                if (boxModel == null)
                {
                    GetModels();
                }

                if (pushableController == null)
                {
                    pushableController = new PushableController();
                    boxData.Bind<BaseObjectModel>().ToValue(boxModel).ForObject(pushableController);
                    boxData.Inject(pushableController);
                }
                
                controllerAddition?.Invoke(pushableController);
            }

            public void GetViews(Action<GameObject> viewAddition = null, Transform viewContainer = null)
            {
                if (boxModel == null)
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
                    boxData.Bind<BaseObjectModel>().ToValue(boxModel).ForGameObject(view);
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
                boxModel = null;
                pushableController = null;
                view = null;
                ObjectPool<BoxPackage>.Put(this);
            }
        }
    }
}