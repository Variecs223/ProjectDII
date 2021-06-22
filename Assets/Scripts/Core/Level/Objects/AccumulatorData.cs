using System;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    [CreateAssetMenu(fileName = "AccumulatorData", menuName = "DII/Core/Objects/Accumulator", order = 2)]
    public class AccumulatorData : BaseObjectData, IFactory<AccumulatorData.AccumulatorPackage>
    {
        [SerializeField] private GameObject accumulatorPrefab;

        public int capacity = 1;
        public float minDistance = 0.2f;
        
        public override void OnPreInjected()
        {
            base.OnPreInjected();

            Bind<AccumulatorData>().ToValue(this);
            Bind<BaseObjectData>().ToValue(this);
            BaseContext.Bind<BaseObjectData>().ToValue(this).ForList();
            Bind<GameObject>().ToValue(accumulatorPrefab).ForType<AccumulatorPackage>();
            Bind<Transform>().ToName(LevelLayoutView.ObjectContainerName).ForType<AccumulatorPackage>();
            MarkAsInjected(accumulatorPrefab);
        }

        AccumulatorPackage IFactory<AccumulatorPackage>.GetInstance()
        {
            return GetConcreteInstance();
        }

        public override IObjectPackage GetInstance()
        {
            return GetConcreteInstance();
        }

        public AccumulatorPackage GetConcreteInstance()
        {
            var package = ObjectPool<AccumulatorPackage>.Get();
            Inject(package);
            return package;
        }
        
        public class AccumulatorPackage: IObjectPackage
        {
            [Inject] private AccumulatorData accumulatorData;
            [Inject] private GameObject viewPrefab;
            [Inject] private Transform viewContainer;
            
            private AccumulatorModel accumulatorModel;
            private ChargeController chargeController;
            private GameObject view;
            
            public void GetModels(Action<BaseObjectModel> modelAddition = null)
            {
                if (accumulatorModel == null)
                {
                    accumulatorModel = CreateInstance<AccumulatorModel>();
                    accumulatorData.Inject(accumulatorModel);
                }
                
                modelAddition?.Invoke(accumulatorModel);
            }

            public void GetControllers(Action<IController> controllerAddition = null)
            {
                if (accumulatorModel == null)
                {
                    GetModels();
                }

                if (chargeController == null)
                {
                    chargeController = new ChargeController();
                    accumulatorData.Bind<AccumulatorModel>().ToValue(accumulatorModel).ForObject(chargeController);
                    accumulatorData.Inject(chargeController);
                }
                
                controllerAddition?.Invoke(chargeController);
            }

            public void GetViews(Action<GameObject> viewAddition = null)
            {
                if (accumulatorModel == null)
                {
                    GetModels();
                }

                if (chargeController == null)
                {
                    GetControllers();
                }

                if (view == null)
                {
                    view = Instantiate(viewPrefab, viewContainer);
                    accumulatorData.Bind<BaseObjectModel>().ToValue(accumulatorModel).ForGameObject(view);
                    accumulatorData.Bind<ChargeController>().ToValue(chargeController).ForGameObject(view);

                    foreach (var injectable in view.GetComponentsInChildren<IInjectable>())
                    {
                        accumulatorData.Inject(injectable);
                    }
                }
                
                viewAddition?.Invoke(view);
            }

            public void Dispose()
            {
                accumulatorData.UnmarkAsInjected(this);
                
                accumulatorData = null;
                viewPrefab = null;
                viewContainer = null;
                accumulatorModel = null;
                chargeController = null;
                view = null;
                ObjectPool<AccumulatorPackage>.Put(this);
            }
        }
    }
}