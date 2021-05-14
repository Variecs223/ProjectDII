using System;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core
{
    [Serializable]
    public class TestModel: IModel
    {
        [field: Inject] public TestModelType TestModelType { get; protected set; }
        ScriptableObject IModel.ModelType => TestModelType;

        public int TestField1;
        public string TestField2;
        

        public void Dispose()
        {
            TestModelType.Dispose();
        }
    }
}