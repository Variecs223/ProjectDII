using System;
using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelInitializer : NameBindingBehaviour, IDisposable
    {
        [SerializeField] private LevelData levelData;
        [SerializeField] private GameObject levelView;
        [SerializeField] private List<InjectorContext> objectDatas;

        private LevelModel model;
        private LevelController controller;
        private LevelLayoutView layoutView;
        
        protected new void Awake()
        {
            base.Awake();

            InjectorContext.BaseContext.Bind<Camera>().ToValue(Camera.main).ForName("MainCamera");
            InjectorContext.BaseContext.Bind<GameObject>().ToValue(levelView).ForType<LevelData>();
            
            levelData.Init();

            foreach (var objectData in objectDatas)
            {
                objectData.ParentContext = levelData;
                objectData.Init();
            }
            
            model = levelData.GetLevelModel();
            controller = levelData.GetLevelController(model);
            layoutView = levelData.GetLevelView(model, controller).GetComponent<LevelLayoutView>();
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
            model.Dispose();
            levelData.Dispose();
            
            foreach (var objectData in objectDatas)
            {
                objectData.Dispose();
            }
        }
    }
}