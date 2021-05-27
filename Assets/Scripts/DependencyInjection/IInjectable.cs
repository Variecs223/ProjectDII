
using System;

namespace Variecs.ProjectDII.DependencyInjection
{
    public interface IInjectable: IDisposable
    {
        void OnInjected();
    }
}