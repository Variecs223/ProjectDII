using System;
using System.Reflection;

namespace Variecs.ProjectDII.DependencyInjection
{
    public interface ICondition: IDisposable
    {
        bool IsFulfilled(object target, FieldInfo fieldInfo);
    }
}