using UnityEngine;
using UnityEngine.EventSystems;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.LevelEditor
{
    public class ObjectSelectionItemView : MonoBehaviour, IInjectable, IPointerClickHandler
    {
        [Inject] [SerializeField] private LevelEditorModel levelEditorModel;
        [SerializeField] private ObjectType targetType = ObjectType.None;

        public void OnInjected()
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            levelEditorModel.SelectedObject = targetType;
        }

        public void Dispose()
        {
            
        }
    }
}