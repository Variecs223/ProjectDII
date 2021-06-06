using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class BaseObjectView : MonoBehaviour, IInjectable
    {
        [Inject] [SerializeField] private BaseObjectModel objectModel;
        [Inject] private LevelModel levelModel;
        [SerializeField] private RectTransform rectTransform;

        public void OnInjected()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
        }
        
        protected void Update()
        {
            var uvCoords = objectModel.coords / levelModel.Data.fieldSize;
            rectTransform.anchorMin = uvCoords;
            rectTransform.anchorMax = uvCoords;
        }

        protected void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            objectModel.Data.UnmarkAsInjected(this);
            objectModel.Data.UnbindGameObject(gameObject);
        }
    }
}