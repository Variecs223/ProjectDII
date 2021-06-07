using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class AccumulatorModel: BaseObjectModel
    {
        [field: Inject] public AccumulatorData ConcreteData { get; }
        
        public int charge;
    }
}