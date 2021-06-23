using System;
using System.Reflection;

namespace Variecs.ProjectDII.DependencyInjection
{
    public interface IBindable<out T>: IDisposable
    {
        T Inject();
        Type GetBindedType();
        bool CheckConditions(object target, FieldInfo field);
        IBindable<T> Clone();
    }
}