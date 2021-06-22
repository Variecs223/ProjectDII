using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.LevelEditor
{
    public class ObjectSelectionItemView : MonoBehaviour, IInjectable, IPointerClickHandler
    {
        [Inject] [SerializeField] private LevelEditorModel levelEditorModel;
        [Inject(Optional=true)] [SerializeField] private BaseObjectData objectData;
        [SerializeField] private Image icon;

        public void OnInjected()
        {
            icon.sprite = objectData.icon;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            levelEditorModel.SelectedObject = objectData.objectType;
        }

        public void Dispose()
        {
            
        }

        public struct Package
        {
            public BaseObjectData ObjectData;
            public ObjectSelectionItemView Prefab;
            public Transform Container;

        }

        public class Factory: IFactory<ObjectSelectionItemView, Package>
        {
            public bool ManuallyInjected => true;
            
            public ObjectSelectionItemView GetInstance(Package package = default)
            {
                var item = Instantiate(package.Prefab, package.Container);
                package.ObjectData.Bind<BaseObjectData>().ToValue(package.ObjectData).ForGameObject(item.gameObject);

                foreach (var injectable in item.GetComponentsInChildren<IInjectable>())
                {
                    package.ObjectData.Inject(injectable);
                }

                return item;
            }
            
            public void Dispose()
            {
                
            }
        }
    }
}