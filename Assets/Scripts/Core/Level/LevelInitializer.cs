using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelInitializer : NameBindingBehaviour
    {
        [SerializeField] private LevelData levelData;
        
        protected new void Awake()
        {
            base.Awake();

            levelData.Init();
        }

        protected void OnDestroy()
        {
            levelData.Dispose();
        }
    }
}