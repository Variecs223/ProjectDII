using System.Linq;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.EndConditions
{
    public class ChargeCollisionLoseCondition: IEndCondition
    {
        [Inject(Name = LevelData.CurrentLevelTag)]
        private LevelModel levelModel;
        
        public bool Check()
        {
            return levelModel.tiles.Any(tile => tile.objects.Any(obj => obj.Object.Data.canCharge
                && tile.objects.Any(otherObj => obj.Object != otherObj.Object 
                    && otherObj.Object.Data.canCharge 
                    && Vector3.Distance(obj.Object.coords, otherObj.Object.coords) <= levelModel.Data.minChargedDistance)));
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