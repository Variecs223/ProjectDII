using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.LevelEditor
{
    public class FlipDirectionView : MonoBehaviour, IInjectable
    {
        [Inject(Name=LevelData.CurrentLevelTag)] private LevelEditorModel levelEditorModel;
       
        public void OnInjected()
        {
            
        }

        public void FlipDirection()
        {
            levelEditorModel.FlipDirection();
        }
        
        public void Dispose()
        {
            levelEditorModel.ModelType.UnmarkAsInjected(this);
            levelEditorModel = null;
        }
    }
}