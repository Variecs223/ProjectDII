using System;
using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Objects;

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

        public List<ObjectTransitionState> objects = new List<ObjectTransitionState>();
        public string test;

        public virtual bool AllowObject(ObjectType type)
        {
            return true;
        }
        
        public virtual void Dispose()
        {
            objects.Clear();
        }
    }
}