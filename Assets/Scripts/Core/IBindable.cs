using System;

namespace Variecs.ProjectDII.Core
{
    public interface IBindable<out T>: IDisposable where T: class
    {
        T Inject();
        bool CheckConditions();
    }
}