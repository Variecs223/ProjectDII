﻿using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    public class LevelController: IController
    {
        [field: Inject] private LevelModel model;
        
        public void Update()
        {
            
        }
    }
}