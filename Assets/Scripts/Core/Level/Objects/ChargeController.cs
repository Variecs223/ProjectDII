using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class ChargeController: IController, IInjectable
    {
        [field: Inject] public AccumulatorData Data { get; protected set; }
        [field: Inject] public AccumulatorModel ObjectModel { get; protected set; }
        [Inject(Name=LevelData.CurrentLevelTag)] private LevelModel levelModel;
        [Inject(Name=LevelData.CurrentLevelTag)] private LevelController levelController;

        public void OnInjected()
        {
            ObjectModel.OnRemoved += Dispose;
        }

        public void Update(float deltaTime)
        {
            var removalList = ObjectPool<List<BaseObjectModel>>.Get();
            
            foreach (var obj in levelModel.objects.Where(obj => obj.Data.canCharge 
                                                                && Vector2.Distance(obj.coords, ObjectModel.coords) <= Data.minDistance))
            {
                ObjectModel.charge++;
                removalList.Add(obj);

                if (levelController.CheckDefeat(EndConditionType.Overcharge) || levelController.CheckVictory(EndConditionType.Accumulator))
                {
                    break;
                }
            }

            foreach (var obj in removalList)
            {
                levelModel.RemoveObject(obj);
            }
        }
        
        public void Dispose()
        {
            levelController?.RemoveController(this);

            if (ObjectModel != null)
            {
                ObjectModel.OnRemoved -= Dispose;

                if (ObjectModel.Data != null)
                {
                    ObjectModel.Data.UnmarkAsInjected(this);
                    ObjectModel.Data.UnbindObject(this);
                }
            }
            
            ObjectModel = null;
            levelModel = null;
            levelController = null;
        }
    }
}