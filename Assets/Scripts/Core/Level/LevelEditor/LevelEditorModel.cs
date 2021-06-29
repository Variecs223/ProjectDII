using System;
using System.Linq;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.LevelEditor
{
    [Serializable]
    public class LevelEditorModel: IModel, IInjectable
    {
        [Inject] [SerializeField] protected LevelModel levelModel;
        
        public InjectorContext ModelType => levelModel.Data;
        
        private ObjectType selectedObject = ObjectType.None;
        private TileType selectedTile = TileType.None;
        private Direction selectedDirection = Direction.Up;

        public ObjectType SelectedObject
        {
            get => selectedObject;
            set
            {
                selectedTile = TileType.None;
                selectedObject = value;
            }
        }

        public TileType SelectedTile
        {
            get => selectedTile;
            set
            {
                selectedObject = ObjectType.None;
                selectedTile = value;
            }
        }

        public Direction SelectedDirection => selectedDirection;

        public event Action OnRemoved;

        public void OnInjected()
        {
            levelModel.OnRemoved += Dispose;
        }

        public void FlipDirection()
        {
            selectedDirection = (Direction) (((int) selectedDirection + 1) % 4);
        }

        public void Save()
        {
            levelModel.Data.tiles = levelModel.tiles.Select(tile => tile.tileType).ToArray();
            levelModel.Data.objects = levelModel.objects.Select(obj => new LevelData.ObjectLocation
            {
                Coords = new Vector2Int(Mathf.FloorToInt(obj.coords.x), Mathf.FloorToInt(obj.coords.y)),
                Direction = obj.direction,
                Type = obj.Data.objectType
            }).ToArray();
            
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        public void Dispose()
        {
            OnRemoved?.Invoke();
            
            levelModel.OnRemoved -= Dispose;
            levelModel = null;
        }
    }
}