using System;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    [Serializable]
    public class LevelModel: IModel
    {
        [field: Inject] public LevelData Data { get; }
        
        ScriptableObject IModel.ModelType => Data;

        public void Dispose()
        {
            Data.UnmarkAsInjected(this);
        }

    }
}