using System;
using UnityEngine;
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

        public event Action OnRemoved;

        public void OnInjected()
        {
            levelModel.OnRemoved += Dispose;
        }

        public void Dispose()
        {
            OnRemoved?.Invoke();
            
            levelModel.OnRemoved -= Dispose;
            levelModel = null;
        }
    }
}