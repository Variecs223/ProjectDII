using System;
using UnityEngine;

namespace Variecs.ProjectDII.Core.Level
{
    [Serializable]
    public class BaseObjectModel: ScriptableObject, IDisposable
    { 
        public virtual void Dispose()
        {
            
        } 
    }
}