using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.LevelEditor
{
    public class LevelEditorInitializer : NameBindingBehaviour, IDisposable
    {
        [SerializeField] private LevelData levelData;
        [SerializeField] private GameObject levelView;
        [SerializeField] private List<BaseObjectData> objectDatas;

        private LevelModel model;
        private LevelEditorModel editorModel;
        private LevelEditorController controller;
        private LevelLayoutView layoutView;
        
        protected new void Awake()
        {
            base.Awake();

            InjectorContext.BaseContext.Bind<Camera>().ToValue(Camera.main).ForName("MainCamera");
            InjectorContext.BaseContext.Bind<GameObject>().ToValue(levelView).ForType<LevelData>();
            InjectorContext.BaseContext.Bind<List<TileType>>()
                .ToValue(Enum.GetValues(typeof(TileType)).Cast<TileType>().Where(type => type != TileType.None).ToList());
            
            levelData.Init();

            foreach (var objectData in objectDatas)
            {
                objectData.ParentContext = levelData;
                objectData.Init();
            }
            
            model = levelData.GetLevelModel();
            editorModel = levelData.GetLevelEditorModel(model);
            controller = levelData.GetLevelEditorController(model, editorModel);
            layoutView = levelData.GetLevelView(model, controller, editorModel).GetComponent<LevelLayoutView>();
            model.Load();
        }

        protected void Update()
        {
            controller.Update(Time.deltaTime);
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
            model?.Dispose();

            if (levelData != null)
            {
                levelData.Dispose();
            }
            
            foreach (var objectData in objectDatas)
            {
                objectData.Dispose();
            }
            
            InjectorContext.BaseContext.Dispose();
        }
    }
}