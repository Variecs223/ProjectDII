using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelLayoutView : MonoBehaviour, IInjectable
    {
        public const string ObjectContainerName = "ObjectContainer";
        
        [Inject] [SerializeField] private LevelModel model;
        [Inject] private LevelController controller;

        [SerializeField] private RawImage levelDisplay;
        [SerializeField] private RectTransform objectContainer;
        [SerializeField] private Vector2Int textureSize;
        [SerializeField] private Vector2 tileSize;
        [Space]
        [SerializeField] private string fieldSizeXParamName = "_FieldSizeX";
        [SerializeField] private string fieldSizeYParamName = "_FieldSizeY";
        [SerializeField] private string textureSizeXParamName = "_TextureSizeX";
        [SerializeField] private string textureSizeYParamName = "_TextureSizeY";
        [SerializeField] private string tileBufferParamName = "_TileBuffer";

        private ComputeBuffer tileBuffer;
        private readonly List<GameObject> objectViews = new List<GameObject>();
        
        public void OnInjected()
        {
            if (levelDisplay == null)
            {
                levelDisplay = GetComponent<RawImage>();
            }

            tileBuffer = new ComputeBuffer(model.Data.tiles.Length, sizeof(int)) { name = "_TileBuffer" };
            tileBuffer.SetData(model.Data.tiles);

            levelDisplay.material.SetInt(textureSizeXParamName, textureSize.x);
            levelDisplay.material.SetInt(textureSizeYParamName, textureSize.y);
            levelDisplay.material.SetInt(fieldSizeXParamName, model.Data.fieldSize.x);
            levelDisplay.material.SetInt(fieldSizeYParamName, model.Data.fieldSize.y);
            levelDisplay.material.SetBuffer(tileBufferParamName, tileBuffer);

            InjectorContext.BaseContext.Bind<Transform>().ToValue(objectContainer).ForName(ObjectContainerName);
            
            model.OnObjectAdded += AddViews;
        }

        protected void AddViews(IObjectPackage package)
        {
            package.GetViews(view =>
            {
                var rectTransform = view.GetComponent<RectTransform>();
                
                objectViews.Add(view);
                
                package.GetModels(objectModel =>
                {
                    var uvCoords = objectModel.coords / model.Data.fieldSize;
                    rectTransform.anchorMin = uvCoords;
                    rectTransform.anchorMax = uvCoords;
                });
            });
        }

        protected void Start()
        {
            var rectTransform = levelDisplay.GetComponent<RectTransform>();

            rectTransform.sizeDelta += new Vector2(
                                           tileSize.x * model.Data.fieldSize.x,
                                           tileSize.y * model.Data.fieldSize.y)
                                       - rectTransform.rect.size;
        }

        protected void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            tileBuffer?.Dispose();

            if (model != null)
            {
                model.OnObjectAdded -= AddViews;

                if (model.Data != null)
                {
                    model.Data.UnmarkAsInjected(this);
                    model.Data.UnbindGameObject(gameObject);
                }
            }
        }
    }
}