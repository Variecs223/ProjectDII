using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core
{
    public class TestView : MonoBehaviour
    {
        [Inject] private TestModel testModel;
        [Inject] private TestController testController;
    }
}