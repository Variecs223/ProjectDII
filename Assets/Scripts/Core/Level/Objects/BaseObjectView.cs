using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class BaseObjectView : MonoBehaviour, IInjectable
    {
        [Inject] [SerializeField] private BaseObjectModel objectModel;
        [Inject] private LevelData levelData;
        [Inject(Name=LevelData.CurrentLevelTag)] private GameObject levelView;
        [SerializeField] private RectTransform rectTransform;

        public void OnInjected()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            objectModel.OnRemoved += DestroyGameObject;
        }
        
        protected void Update()
        {
            var uvCoords = objectModel.coords / levelData.fieldSize;
            rectTransform.anchorMin = uvCoords;
            rectTransform.anchorMax = uvCoords;

            rectTransform.rotation = Quaternion.AngleAxis(90 * (int) objectModel.direction, Vector3.forward);
        }

        protected void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            objectModel.OnRemoved -= DestroyGameObject;
            
            var levelLayoutView = levelView.GetComponent<LevelLayoutView>();

            if (levelLayoutView != null)
            {
                levelLayoutView.RemoveView(gameObject);
            }
            
            objectModel.Data.UnmarkAsInjected(this);
            objectModel.Data.UnbindGameObject(gameObject);
        }

        private void DestroyGameObject()
        {
            Destroy(gameObject);
        }
    }
}