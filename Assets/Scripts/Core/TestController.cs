using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core
{
    public class TestController: IController
    {
        [Inject] private TestModel model;
        
        public void Update()
        {
            
        }
    }
}