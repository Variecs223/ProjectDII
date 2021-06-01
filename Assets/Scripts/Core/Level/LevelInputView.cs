using UnityEngine;
using UnityEngine.EventSystems;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelInputView : MonoBehaviour, IInjectable, IPointerClickHandler
    {
        [Inject] [SerializeField] private LevelModel model;
        [Inject] private LevelController controller;
        [Inject(Name="MainCamera")] private Camera activeCamera;

        [SerializeField] private RectTransform touchScreen;

        protected void Awake()
        {
            if (touchScreen == null)
            {
                touchScreen = GetComponent<RectTransform>();
            }
        }
        
        public void OnInjected()
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var positions = new Vector3[4];
            var worldPoint = activeCamera.ScreenToWorldPoint(eventData.position);
            
            touchScreen.GetWorldCorners(positions);

            var coords = new Vector2(
                Mathf.InverseLerp(positions[0].x, positions[2].x, worldPoint.x),
                Mathf.InverseLerp(positions[0].y, positions[2].y, worldPoint.y));
            coords *= model.Data.fieldSize;
            
            controller.OnTileClick(new Vector2Int(Mathf.FloorToInt(coords.x), Mathf.FloorToInt(coords.y)));
        }
        
        protected void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (model?.Data != null)
            {
                model.Data.UnmarkAsInjected(this);
            }
        }
    }
}