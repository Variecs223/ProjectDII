using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core
{
    [CreateAssetMenu(fileName = "TestModelType", menuName = "Model Types/Test Model Type", order = 0)]
    public class TestModelType : InjectorContext
    {
        [SerializeField] private TestView viewPrefab;
        
        [Inject] private IFactory<TestModel> testModelFactory;
        [Inject] private IFactory<TestController, TestModel> testControllerFactory;
        [Inject] private IFactory<TestView, TestViewFactoryArgs> testViewFactory;
        
        protected new void Awake()
        {
            base.Awake();

            Bind<TestModelType>().ToValue(this);
            Bind<IFactory<TestModel>>().ToSingleton<TestModelFactory>();
            Bind<IFactory<TestController, TestModel>>().ToSingleton<TestControllerFactory>();
            Bind<IFactory<TestView, TestViewFactoryArgs>>().ToSingleton<TestViewFactory>();
            Bind<TestView>().ToValue(viewPrefab).ForType<TestViewFactory>();
            Bind<Transform>().ForType<TestViewFactory>();
            
            Inject(this);
        }

        public TestModel GetTestModel()
        {
            return testModelFactory.GetInstance();
        }

        public TestController GetTestController(TestModel model)
        {
            return testControllerFactory.GetInstance(model);
        }

        public TestView GetView(TestViewFactoryArgs args)
        {
            return testViewFactory.GetInstance(args);
        }

        public class TestModelFactory: IFactory<TestModel>
        {
            public bool ManuallyInjected => true;
            [field: Inject] public InjectorContext Context { get; }
            
            public TestModel GetInstance()
            {
                var model = new TestModel();
                Context.Inject(model);
                return model;
            }

            public void Dispose()
            {
                
            }
        }
        
        public class TestControllerFactory : IFactory<TestController, TestModel> 
        {
            public bool ManuallyInjected => true;
            [field: Inject] public TestModelType ModelType { get; }

            public TestController GetInstance(TestModel model)
            {
                var controller = new TestController();
                ModelType.Bind<TestModel>().ToValue(model).SetTemporary();
                ModelType.Inject(controller);
                return controller;
            }

            public void Dispose()
            {
                
            }
        }
        
        public class TestViewFactory : IFactory<TestView, TestViewFactoryArgs> 
        {
            public bool ManuallyInjected => true;
            [field: Inject] public TestModelType ModelType { get; }
            [field: Inject] public TestView ViewPrefab { get; }
            [field: Inject] public Transform ViewParent { get; }

            public TestView GetInstance(TestViewFactoryArgs args)
            {
                var view = Instantiate(ViewPrefab, ViewParent);
                ModelType.Bind<TestModel>().ToValue(args.Model).SetTemporary();
                ModelType.Bind<TestController>().ToValue(args.Controller).SetTemporary();
                ModelType.Inject(view);
                return view;
            }

            public void Dispose()
            {
                
            }
        }

        public struct TestViewFactoryArgs
        {
            public TestModel Model;
            public TestController Controller;
        }
    }
}