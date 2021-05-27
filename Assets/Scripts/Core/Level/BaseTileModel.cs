using System;
using System.Collections.Generic;
using UnityEngine;

namespace Variecs.ProjectDII.Core.Level
{
    [Serializable]
    public class BaseTileModel: IDisposable
    {
        [Serializable]
        public struct ObjectTransitionState
        {
            public BaseObjectModel Object;
            public TransitionState State;
        }

        public List<ObjectTransitionState> objects;
        public string test;

        public virtual void Dispose()
        {
            
        }
    }
}