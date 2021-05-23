using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelView : MonoBehaviour, IInjectable
    {
        [Inject] [SerializeField] private LevelModel model;
        [Inject] private LevelController controller;
        
        public void OnInjected()
        {
            
        }
    }
}