using UnityEngine;
using UnityEngine.UI;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelView : MonoBehaviour, IInjectable
    { 
        [Inject] [SerializeField] private LevelModel model;
        [Inject] private LevelController controller;

        [SerializeField] private RawImage levelDisplay;
        [SerializeField] private string sizeXParamName = "_SizeX";
        [SerializeField] private string sizeYParamName = "_SizeY";
        [SerializeField] private string tileBufferParamName = "_TileBuffer";

        private ComputeBuffer tileBuffer;
        
        protected void Awake()
        {
            if (levelDisplay == null)
            {
                levelDisplay = GetComponent<RawImage>();
            }

            tileBuffer = new ComputeBuffer(model.Data.tiles.Length, sizeof(int)) { name = "_TileBuffer" };
            tileBuffer.SetData(model.Data.tiles);

            levelDisplay.material.SetInt(sizeXParamName, model.Data.size.x);
            levelDisplay.material.SetInt(sizeYParamName, model.Data.size.y);
            levelDisplay.material.SetBuffer(tileBufferParamName, tileBuffer);
        }
        
        public void OnInjected()
        {
            
        }

        protected void OnDestroy()
        {
            tileBuffer?.Dispose();
        }
    }
}