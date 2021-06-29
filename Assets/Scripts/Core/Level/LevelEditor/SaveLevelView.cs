using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.LevelEditor
{
    public class SaveLevelView : MonoBehaviour, IInjectable
    {
        [Inject(Name=LevelData.CurrentLevelTag)] private LevelEditorModel levelEditorModel;
       
        public void OnInjected()
        {
            
        }

        public void SaveLevel()
        {
            levelEditorModel.Save();
        }
        
        public void Dispose()
        {
            levelEditorModel.ModelType.UnmarkAsInjected(this);
            levelEditorModel = null;
        }
    }
}