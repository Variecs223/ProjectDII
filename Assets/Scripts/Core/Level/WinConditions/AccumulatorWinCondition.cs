using System.Linq;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.WinConditions
{
    public class AccumulatorWinCondition: IEndCondition
    {
        [Inject(Name = LevelData.CurrentLevelTag)]
        private LevelModel levelModel;
        
        public bool Check()
        {
            return levelModel.objects.All(obj =>
                !(obj is AccumulatorModel accumulator) || accumulator.charge == accumulator.ConcreteData.capacity);
        }
        
        public void Dispose()
        {
            if (levelModel?.Data != null)
            {
                levelModel.Data.UnmarkAsInjected(this);    
                levelModel.Data.UnbindObject(this);    
            }
            
            levelModel = null;
        }
    }
}