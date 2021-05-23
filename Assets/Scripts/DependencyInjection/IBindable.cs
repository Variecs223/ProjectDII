using System;
using System.Reflection;

namespace Variecs.ProjectDII.DependencyInjection
{
    public interface IBindable<out T>: IDisposable where T: class
    {
        T Inject();
        bool CheckConditions(object target, FieldInfo field);
        IBindable<T> Clone();
        bool Temporary { get; }
    }
}