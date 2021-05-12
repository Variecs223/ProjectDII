using System;

namespace Variecs.ProjectDII.DependencyInjection
{
    public interface IBindable<out T>: IDisposable where T: class
    {
        T Inject();
        bool CheckConditions();
    }
}