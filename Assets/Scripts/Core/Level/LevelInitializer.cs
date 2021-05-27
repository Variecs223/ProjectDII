using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelInitializer : NameBindingBehaviour
    {
        [SerializeField] private LevelData levelData;

        private LevelModel model;
        private LevelController controller;
        private LevelLayoutView layoutView;
        
        protected new void Awake()
        {
            base.Awake();

            levelData.Init();
            model = levelData.GetLevelModel();
            controller = levelData.GetLevelController(model);
            layoutView = levelData.GetLevelView(model, controller).GetComponent<LevelLayoutView>();
        }

        protected void OnDestroy()
        {
            
            controller.Dispose();
            model.Dispose();
            levelData.Dispose();
        }
    }
}