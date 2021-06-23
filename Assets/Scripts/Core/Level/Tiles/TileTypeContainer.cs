using System;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Tiles
{
    [Serializable]
    public class TileTypeContainer: IDisposable
    {
        public TileType type;

        public void Dispose()
        {
            ObjectPool<TileTypeContainer>.Put(this);
        }
    }
}