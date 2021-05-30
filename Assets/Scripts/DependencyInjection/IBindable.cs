using System;
using System.Reflection;

namespace Variecs.ProjectDII.DependencyInjection
{
    public interface IBindable<out T>: IDisposable where T: class
    {
        T Inject();
        Type GetBindedType();
        bool CheckConditions(object target, FieldInfo field);
        IBindable<T> Clone();
    }
}