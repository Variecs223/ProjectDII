using System;
using System.Collections.Generic;
using System.Linq;
using Variecs.ProjectDII.Core.Level.Objects;

namespace Variecs.ProjectDII.Core.Level.Tiles
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

        public TileType tileType;
        public List<ObjectTransitionState> objects = new List<ObjectTransitionState>();

        public virtual bool AllowObject(ObjectType type)
        {
            return objects.All(obj => obj.Object.AllowObject(type));
        }
        
        public virtual void Dispose()
        {
            objects.Clear();
        }
    }
}