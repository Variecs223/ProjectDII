using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.LevelEditor
{
    public class TileSelectionItemView : MonoBehaviour, IInjectable, IPointerClickHandler
    {
        private static int Counter = 1;
        
        [Inject] [SerializeField] private LevelEditorModel levelEditorModel;
        [Inject(Optional=true)] [SerializeField] private TileTypeContainer tileTypeContainer;
        [SerializeField] private RawImage icon;
        [SerializeField] private Vector2Int textureSize;
        [Space]
        [SerializeField] private string fieldSizeXParamName = "_FieldSizeX";
        [SerializeField] private string fieldSizeYParamName = "_FieldSizeY";
        [SerializeField] private string textureSizeXParamName = "_TextureSizeX";
        [SerializeField] private string textureSizeYParamName = "_TextureSizeY";
        [SerializeField] private string tileBufferParamName = "_TileBuffer";

        private ComputeBuffer tileBuffer;

        protected void Awake()
        {
            icon.material = new Material(icon.material);
        }
        
        public void OnInjected()
        {
            tileBuffer = new ComputeBuffer(1, sizeof(int)) { name = $"{tileBufferParamName}/{Counter++}" };
            
            UpdateIcon();
        }

        public void UpdateIcon()
        {
            tileBuffer.SetData(new[] { tileTypeContainer.type });

            icon.material.SetInt(textureSizeXParamName, textureSize.x);
            icon.material.SetInt(textureSizeYParamName, textureSize.y);
            icon.material.SetInt(fieldSizeXParamName, 1);
            icon.material.SetInt(fieldSizeYParamName, 1);
            icon.material.SetBuffer(tileBufferParamName, tileBuffer);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            levelEditorModel.SelectedTile = tileTypeContainer.type;
        }

        protected void OnDestroy()
        {
            Dispose();
        }

        protected void OnApplicationQuit()
        {
            Dispose();
        }

        public void Dispose()
        {
            tileTypeContainer?.Dispose();
            tileBuffer?.Dispose();
        }

        public struct Package
        {
            public TileType Type;
            public TileSelectionItemView Prefab;
            public Transform Container;

        }

        public class Factory: IFactory<TileSelectionItemView, Package>
        {
            [Inject] private InjectorContext context;
            
            public bool ManuallyInjected => true;
            
            public TileSelectionItemView GetInstance(Package package = default)
            {
                var item = Instantiate(package.Prefab, package.Container);
                var container = ObjectPool<TileTypeContainer>.Get();
                container.type = package.Type;
                
                context.Bind<TileTypeContainer>().ToValue(container).ForGameObject(item.gameObject);

                foreach (var injectable in item.GetComponentsInChildren<IInjectable>())
                {
                    context.Inject(injectable);
                }

                return item;
            }
            
            public void Dispose()
            {
                context = null;
            }
        }
    }
}