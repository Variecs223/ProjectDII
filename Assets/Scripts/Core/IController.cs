using System;

namespace Variecs.ProjectDII.Core
{
    public interface IController: IDisposable
    {
        void Update(float deltaTime);
    }
}