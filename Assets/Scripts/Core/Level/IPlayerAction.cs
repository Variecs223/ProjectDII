using System;
using UnityEngine;

namespace Variecs.ProjectDII.Core.Level
{
    public interface IPlayerAction: IDisposable
    {
        public bool Perform(Vector2Int coords);
    }
}