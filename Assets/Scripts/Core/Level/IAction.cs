using System;
using UnityEngine;

namespace Variecs.ProjectDII.Core.Level
{
    public interface IAction: IDisposable
    {
        public bool Perform(Vector2Int coords);
    }
}